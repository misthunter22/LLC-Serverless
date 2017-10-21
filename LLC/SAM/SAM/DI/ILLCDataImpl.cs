using Amazon.DynamoDBv2;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;
using System.Collections.Generic;
using System.Linq;
using Amazon;
using SAM.Models.Admin;
using System;
using SAM.Models.Reports;
using SAM.Models;
using Newtonsoft.Json;
using System.IO;
using Amazon.DynamoDBv2.DataModel;
using SAM.Models.Dynamo;
using Amazon.S3.Model;
using Amazon.S3;
using Amazon.DynamoDBv2.DocumentModel;

namespace SAM.DI
{
    public class ILLCDataImpl : ILLCData
    {
        protected RegionEndpoint _region = RegionEndpoint.USWest2;

        private string _bucketTableName = "LLC-Buckets";

        public RegionEndpoint Region()
        {
            return _region;
        }

        public long TableCount(string tableName)
        {
            using (var client = new AmazonDynamoDBClient(_region))
            {
                var descr = client.DescribeTableAsync(tableName);
                var count = descr.Result.Table.ItemCount;
                return count;
            }
        }

        public List<Sources> Sources()
        {
            using (var client = new AmazonDynamoDBClient(_region))
            {
                using (var ctx = new DynamoDBContext(client))
                {
                    var sources = ctx.FromScanAsync<Sources>(new ScanOperationConfig()).GetRemainingAsync().Result;
                    foreach (var m in sources)
                    {
                        // Skip the internal sources
                        if (m.Id > 0)
                        {
                            m.S3ObjectName = QueryDataAttribute(_bucketTableName, m.S3BucketId.ToString(), "Name").Result.S;
                            m.S3BucketSearchPrefix = QueryDataAttribute(_bucketTableName, m.S3BucketId.ToString(), "SearchPrefix").Result.S;
                            Console.WriteLine($"S3 object name is {m.S3ObjectName}");
                        }
                    }

                    return sources.OrderBy(x => x.Id).ToList();
                }
            }
        }

        public Sources Source(string id, SourceSearchType type)
        {
            using (var client = new AmazonDynamoDBClient(_region))
            {
                var results = Sources();
                switch(type)
                {
                    case SourceSearchType.Id:
                        return results.FirstOrDefault(x => x.Id.Equals(id));
                    case SourceSearchType.Name:
                        return results.FirstOrDefault(x => id.Equals(x.S3ObjectName, StringComparison.CurrentCultureIgnoreCase));
                    default:
                        return null;
                }
            }
        }

        public List<Settings> Settings()
        {
            using (var client = new AmazonDynamoDBClient(_region))
            {
                using (var ctx = new DynamoDBContext(client))
                {
                    var settings = ctx.FromScanAsync<Settings>(new ScanOperationConfig()).GetRemainingAsync().Result;
                    return settings.OrderBy(x => x.Name).ToList();
                }
            }
        }

        public List<InvalidLinks> InvalidLinks()
        {
            using (var client = new AmazonDynamoDBClient(_region))
            {
                using (var ctx = new DynamoDBContext(client))
                {
                    var links = ctx.FromScanAsync<InvalidLinks>(new ScanOperationConfig
                    {
                        FilterExpression = new Expression
                        {
                            ExpressionStatement = "ReportType = :val",
                            ExpressionAttributeValues = new Dictionary<string, DynamoDBEntry> {
                                { ":val",  "Invalid" }
                            }
                        }
                    })
                    .GetRemainingAsync().Result;
                    return links;
                }
            }
        }

        public List<WarningLinks> WarningLinks()
        {
            using (var client = new AmazonDynamoDBClient(_region))
            {
                using (var ctx = new DynamoDBContext(client))
                {
                    var links = ctx.FromScanAsync<WarningLinks>(new ScanOperationConfig
                    {
                        FilterExpression = new Expression
                        {
                            ExpressionStatement = "ReportType = :val",
                            ExpressionAttributeValues = new Dictionary<string, DynamoDBEntry> {
                                { ":val",  "Warning" }
                            }
                        }
                    })
                    .GetRemainingAsync().Result;
                    return links;
                }
            }
        }

        public void AddUrlToWarningLinks(List<WarningLinks> links)
        {
            using (var client = new AmazonDynamoDBClient(_region))
            {
                var keyList = new List<Dictionary<string, AttributeValue>>();
                foreach (var r in links)
                {
                    keyList.Add(new Dictionary<string, AttributeValue>
                    {
                        { "Id", new AttributeValue { N = r.LinkId.ToString() } }
                    });
                }

                var batch = client.BatchGetItemAsync(new BatchGetItemRequest
                {
                    RequestItems = new Dictionary<string, KeysAndAttributes>
                    {
                        {
                            "LLC-Links", new KeysAndAttributes
                            {
                                AttributesToGet = new List<string> { "Id", "Url" },
                                ConsistentRead = true,
                                Keys = keyList
                            }
                        }
                    }
                });

                var writer = new StringWriter();
                JsonSerializer.Create().Serialize(writer, batch.Result);
                Console.WriteLine(writer.ToString());

                foreach (var r in batch.Result.Responses["LLC-Links"])
                {
                    var first = links.FirstOrDefault(x => x.LinkId == ParseInt(r["Id"].N));
                    if (first != null)
                    {
                        first.Url = r["Url"].S;
                    }
                }
            }
        }

        public List<BucketLocationsModel> BucketLocations(BucketLocationsRequest m, string objectLinksTable, string objectsTable, string statsTable)
        {
            using (var client = new AmazonDynamoDBClient(_region))
            {
                var buckets = Buckets();

                var id = m.id;
                if ("stat".Equals(m.type))
                {
                    id = QueryDataAttribute(statsTable, id, "Link").Result.N;
                }

                var ret = new List<string>();
                var last = new Dictionary<string, AttributeValue>();
                last["Link"] = new AttributeValue { N = "0" };

                while (last.ContainsKey("Link"))
                {
                    var rows = client.ScanAsync(new ScanRequest
                    {
                        ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                            { ":val",  new AttributeValue { N = id } }
                        },
                        FilterExpression = "Link = :val",
                        TableName = objectLinksTable,
                        ExclusiveStartKey = last["Link"].N == "0" ? null : last
                    }).Result;

                    last = rows.LastEvaluatedKey;
                    ret.AddRange(rows.Items.Select(x => x.ContainsKey("Object") ? x["Object"].N : "").ToList());
                }

                var resp = new List<BucketLocationsModel>();
                foreach (var o in ret)
                {
                    var obj = client.QueryAsync(new QueryRequest
                    {
                        TableName = objectsTable,
                        KeyConditionExpression = "Id = :v_Id",
                        ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                            { ":v_Id", new AttributeValue { N =  o }}
                        }
                    });

                    var item = obj.Result.Items.FirstOrDefault();
                    resp.Add(new BucketLocationsModel
                    {
                        data = string.Format("https://{0}.s3.amazonaws.com/{1}", buckets.FirstOrDefault(x => x.Id == ParseInt(item["Bucket"].N)).Name, item["Key"].S)
                    });
                }

                return resp;
            }
        }

        public List<Buckets> Buckets()
        {
            using (var client = new AmazonDynamoDBClient(_region))
            {
                using (var ctx = new DynamoDBContext(client))
                {
                    var buckets = ctx.FromScanAsync<Buckets>(new ScanOperationConfig()).GetRemainingAsync().Result;
                    return buckets;
                }
            }
        }

        public GetObjectResponse ObjectGet(string bucket, string key)
        {
            using (var client = new AmazonS3Client(_region))
            {
                return client.GetObjectAsync(new GetObjectRequest
                {
                    BucketName = bucket,
                    Key = key
                }).Result;
            }
        }

        public async Task<long> IncrementMetaTableKey(string key, long diff)
        {
            using (var client = new AmazonDynamoDBClient(_region))
            {
                using (var ctx = new DynamoDBContext(client))
                {
                    while (true)
                    {
                        try
                        {
                            var meta = ctx.LoadAsync<Meta>(key).Result;

                            Console.WriteLine($"Key before: {meta.Key}");
                            meta.Key = meta.Key + diff;

                            await ctx.SaveAsync(meta);

                            Console.WriteLine($"Key after : {meta.Key}");
                            return meta.Key;
                        }
                        catch (ConditionalCheckFailedException)
                        {
                            Console.WriteLine("ConditionalCheckFailedException. Retrying again...");
                        }
                    }
                }
            }
        }

        public async Task<long> SetMetaTableKey(string key, long set)
        {
            using (var client = new AmazonDynamoDBClient(_region))
            {
                using (var ctx = new DynamoDBContext(client))
                {
                    while (true)
                    {
                        try
                        {
                            var meta = ctx.LoadAsync<Meta>(key).Result;

                            Console.WriteLine($"Key before: {meta.Key}");
                            meta.Key = set;

                            await ctx.SaveAsync(meta);

                            Console.WriteLine($"Key after : {meta.Key}");
                            return meta.Key;
                        }
                        catch (ConditionalCheckFailedException)
                        {
                            Console.WriteLine("ConditionalCheckFailedException. Retrying again...");
                        }
                    }
                }
            }
        }

        public async Task<string> QueryCountBool(string tableName, string column, bool b)
        {
            using (var client = new AmazonDynamoDBClient(_region))
            {
                var rows = await client.ScanAsync(new ScanRequest
                {
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                        { ":val",  new AttributeValue { BOOL = b } }
                    },
                    FilterExpression = $"{column} = :val",
                    TableName = tableName
                });

                return rows.Count.ToString();
            }
        }

        public async Task<AttributeValue> QueryDataAttribute(string tableName, string key, string field)
        {
            var dict = await QueryDataAttributes(tableName, key);
            AttributeValue ret = null;
            if (dict != null)
            {
                ret = dict[field];
            }

            return ret;
        }

        public async Task<Dictionary<string, AttributeValue>> QueryDataAttributes(string tableName, string key)
        {
            using (var client = new AmazonDynamoDBClient(_region))
            {
                var resp = await client.QueryAsync(new QueryRequest
                {
                    TableName = tableName,
                    KeyConditionExpression = "Id = :v_Id",
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                        { ":v_Id", new AttributeValue { N =  key }}
                    }
                });

                var dict = resp.Items.FirstOrDefault();
                return dict;
            }
        }

        public async Task<string> QueryCountContains(string tableName, string column, string s)
        {
            using (var client = new AmazonDynamoDBClient(_region))
            {
                var rows = await client.ScanAsync(new ScanRequest
                {
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                        { ":val",  new AttributeValue(s) }
                    },
                    FilterExpression = $"contains({column}, :val)",
                    TableName = tableName
                });

                return rows.Count.ToString();
            }
        }

        public async Task<List<Dictionary<string, AttributeValue>>> QueryTableAll(string tableName)
        {
            using (var client = new AmazonDynamoDBClient(_region))
            {
                var rows = await client.ScanAsync(new ScanRequest
                {
                    TableName = tableName
                });

                return rows.Items;
            }
        }

        private string ParseDate(string date)
        {
            DateTime ret = DateTime.MinValue;
            string str   = null;
            if (DateTime.TryParse(date, out ret))
            {
                str = ret.ToString("f");
                return str;
            }

            return null;
        }

        private int ParseInt(string i)
        {
            int ret = -1;
            int.TryParse(i, out ret);
            return ret;
        }

        private double ParseDouble(string i)
        {
            double ret = -1.0;
            double.TryParse(i, out ret);
            return ret;
        }

        private long ParseLong(string i)
        {
            long ret = -1;
            long.TryParse(i, out ret);
            return ret;
        }
    }
}
