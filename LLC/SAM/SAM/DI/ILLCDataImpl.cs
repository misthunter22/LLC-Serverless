using Amazon.DynamoDBv2;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;
using System.Collections.Generic;
using System.Linq;
using Amazon;
using SAM.Models.Admin;
using System;
using SAM.Models.Reports;
using Newtonsoft.Json;
using System.IO;
using Amazon.DynamoDBv2.DataModel;
using SAM.Models.Dynamo;
using Amazon.S3.Model;
using Amazon.S3;
using Amazon.DynamoDBv2.DocumentModel;
using System.Text;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace SAM.DI
{
    public class ILLCDataImpl : ILLCData
    {
        protected RegionEndpoint _region = RegionEndpoint.USWest2;

        private string _bucketTableName = "LLC-Buckets";

        private int _maxQueue = 10;

        public RegionEndpoint Region()
        {
            return _region;
        }

        private static void WaitUntilTableReady(AmazonDynamoDBClient client, string tableName)
        {
            string status = null;
            // Let us wait until table is created. Call DescribeTable.
            do
            {
                System.Threading.Thread.Sleep(5000); // Wait 5 seconds.
                try
                {
                    var res = client.DescribeTableAsync(new DescribeTableRequest
                    {
                        TableName = tableName
                    });

                    Console.WriteLine("Table name: {0}, status: {1}",
                              res.Result.Table.TableName,
                              res.Result.Table.TableStatus);
                    status = res.Result.Table.TableStatus;
                }
                catch (ResourceNotFoundException)
                {
                    // DescribeTable is eventually consistent. So you might
                    // get resource not found. So we handle the potential exception.
                }
            } while (status != "ACTIVE");
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

        public void TableCapacity(string tableName, int read, int write)
        {
            if (read <= 0 && write <= 0)
                return;

            using (var client = new AmazonDynamoDBClient(_region))
            {
                ProvisionedThroughput throughput;

                if (read > 0 && write <= 0)
                {
                    throughput = new ProvisionedThroughput
                    {
                        ReadCapacityUnits = read,
                        WriteCapacityUnits = 5
                    };
                }
                else if (read <= 0 && write > 0)
                {
                    throughput = new ProvisionedThroughput
                    {
                        ReadCapacityUnits = 5,
                        WriteCapacityUnits = write
                    };
                }
                else
                {
                    throughput = new ProvisionedThroughput
                    {
                        ReadCapacityUnits = read,
                        WriteCapacityUnits = write
                    };
                }

                var descr = client.UpdateTableAsync(new UpdateTableRequest
                {
                    ProvisionedThroughput = throughput,
                    TableName = tableName
                });

                WaitUntilTableReady(client, tableName);
            }
        }

        public Objects Object(string id)
        {
            using (var client = new AmazonDynamoDBClient(_region))
            {
                using (var ctx = new DynamoDBContext(client))
                {
                    return ctx.LoadAsync<Objects>(id).Result;
                }
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
                        if (m.S3BucketId != null)
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
                switch (type)
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

        public Settings Settings(string name)
        {
            var settings = Settings();
            return settings.FirstOrDefault(x => x.Name == name);
        }

        public List<InvalidLinks> InvalidLinks()
        {
            return GetTableScan<InvalidLinks>("ReportType", "Invalid");
        }

        public List<WarningLinks> WarningLinks()
        {
            return GetTableScan<WarningLinks>("ReportType", "Warning");
        }

        public void AddUrlToWarningLinks(List<WarningLinks> links)
        {
            using (var client = new AmazonDynamoDBClient(_region))
            {
                var keyList = new List<Dictionary<string, AttributeValue>>();
                foreach (var r in links)
                {
                    if (!string.IsNullOrEmpty(r.Link))
                    {
                        keyList.Add(new Dictionary<string, AttributeValue>
                        {
                            { "Id", new AttributeValue { S = r.Link } }
                        });
                    }
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
                    var first = links.FirstOrDefault(x => x.Link == r["Id"].S);
                    if (first != null)
                    {
                        first.Url = r["Url"].S;
                    }
                }
            }
        }

        public void AddLink(Links link)
        {
            using (var client = new AmazonDynamoDBClient(_region))
            {
                using (var ctx = new DynamoDBContext(client))
                {
                    
                }
            }
        }

        public List<BucketLocationsModel> BucketLocations(BucketLocationsRequest m)
        {
            using (var client = new AmazonDynamoDBClient(_region))
            {
                var buckets = Buckets();

                var id = m.id;
                if ("stat".Equals(m.type))
                {
                    id = QueryDataAttribute("LLC-Stats", id, "Link").Result.S;
                }

                var ret = new List<string>();
                var last = new Dictionary<string, AttributeValue>();
                last["Link"] = new AttributeValue { S = "0" };

                while (last.ContainsKey("Link"))
                {
                    var rows = client.ScanAsync(new ScanRequest
                    {
                        ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                            { ":val",  new AttributeValue { S = id } }
                        },
                        FilterExpression = "Link = :val",
                        TableName = "LLC-ObjectLinks",
                        ExclusiveStartKey = last["Link"].S == "0" ? null : last
                    }).Result;

                    last = rows.LastEvaluatedKey;
                    ret.AddRange(rows.Items.Select(x => x.ContainsKey("Object") ? x["Object"].S : "").ToList());
                }

                var resp = new List<BucketLocationsModel>();
                foreach (var o in ret)
                {
                    var obj = client.QueryAsync(new QueryRequest
                    {
                        TableName = "LLC-Objects",
                        KeyConditionExpression = "Id = :v_Id",
                        ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                            { ":v_Id", new AttributeValue { S =  o }}
                        }
                    });

                    var item = obj.Result.Items.FirstOrDefault();
                    resp.Add(new BucketLocationsModel
                    {
                        data = string.Format("https://{0}.s3.amazonaws.com/{1}", buckets.FirstOrDefault(x => x.Id == item["Bucket"].S).Name, item["Key"].S)
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

        public int LinkExtractor(string bucket)
        {
            using (var client = new AmazonDynamoDBClient(_region))
            {
                using (var ctx = new DynamoDBContext(client))
                {
                    var count     = 0;
                    var completed = false;

                    var row = ctx.FromScanAsync<Objects>(new ScanOperationConfig
                    {
                        FilterExpression = new Expression
                        {
                            ExpressionStatement = "#bucket = :bucket and attribute_not_exists(#disabled) and contains(#key, :html) and (attribute_not_exists(#date) or #date < #content)",
                            ExpressionAttributeNames = new Dictionary<string, string> {
                                { "#bucket",   "Bucket" },
                                { "#disabled", "LinkCheckDisabledDate" },
                                { "#key",      "Key" },
                                { "#date",     "DateLinksLastExtracted" },
                                { "#content",  "ContentLastModified" }
                            },
                            ExpressionAttributeValues = new Dictionary<string, DynamoDBEntry> {
                                { ":bucket", bucket },
                                { ":html",   ".htm" }
                            }
                        }
                    });

                    while (!completed)
                    {
                        var result  = row.GetNextSetAsync();
                        var objects = result.Result.Where(x => x.IsFolder == false).ToList();
                        EnqueueObjects(objects);
                        completed = row.IsDone;
                        count += objects.Count;
                    }

                    return count;
                }
            }
        }

        public void EnqueueObjects(List<Objects> objects)
        {
            using (var sqsClient = new AmazonSQSClient())
            {
                var count = objects.Count < _maxQueue ? 1 : (int) Math.Ceiling(((double)objects.Count) / ((double)_maxQueue));
                for (var i = 0; i < count; i++)
                {
                    sqsClient.SendMessageBatchAsync(new SendMessageBatchRequest
                    {
                        Entries = objects.Skip(i * _maxQueue).Take(_maxQueue).Select(x => new SendMessageBatchRequestEntry
                        {
                            Id = Guid.NewGuid().ToString(),
                            MessageBody = JsonConvert.SerializeObject(x)
                        })
                        .ToList(),
                        QueueUrl = Environment.GetEnvironmentVariable("Queue")
                    });
                }
            }
        }

        public List<Objects> DequeueObjects()
        {
            using (var sqsClient = new AmazonSQSClient())
            {
                var list = new List<Objects>();

                var messages = sqsClient.ReceiveMessageAsync(new ReceiveMessageRequest
                {
                    MaxNumberOfMessages = _maxQueue,
                    QueueUrl = Environment.GetEnvironmentVariable("Queue")
                });

                foreach (var message in messages.Result.Messages)
                {
                    var obj = JsonConvert.DeserializeObject<Objects>(message.Body);
                    obj.ReceiptHandle = message.ReceiptHandle;
                    list.Add(obj);
                }

                return list;
            }
        }

        public void RemoveObjectsFromQueue(List<Objects> objects)
        {
            using (var sqsClient = new AmazonSQSClient())
            {
                sqsClient.DeleteMessageBatchAsync(new DeleteMessageBatchRequest
                {
                    Entries = objects.Select(x => new DeleteMessageBatchRequestEntry
                    {
                        Id = Guid.NewGuid().ToString(),
                        ReceiptHandle = x.ReceiptHandle
                    }).ToList(),
                    QueueUrl = Environment.GetEnvironmentVariable("Queue")
                });
            }
        }

        public bool QueueEmpty()
        {
            using (var sqsClient = new AmazonSQSClient())
            {
                var attr = sqsClient.GetQueueAttributesAsync(new GetQueueAttributesRequest
                {
                    AttributeNames = new List<string> { "ApproximateNumberOfMessages" },
                    QueueUrl = Environment.GetEnvironmentVariable("Queue")
                });

                Console.WriteLine($"Queue count: {0}", attr.Result.ApproximateNumberOfMessages);
                return attr.Result.ApproximateNumberOfMessages == 0;
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

        public PutObjectResponse ObjectPut<T>(string bucket, string key, T obj)
        {
            using (var client = new AmazonS3Client(_region))
            {
                return client.PutObjectAsync(new PutObjectRequest
                {
                    BucketName = bucket,
                    Key = key,
                    ContentType = "application/json",
                    InputStream = new MemoryStream(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(obj)))
                }).Result;
            }
        }

        public async Task<T> SetTableRow<T>(T row)
        {
            using (var client = new AmazonDynamoDBClient(_region))
            {
                using (var ctx = new DynamoDBContext(client))
                {
                    while (true)
                    {
                        try
                        {
                            await ctx.SaveAsync(row);
                            return row;
                        }
                        catch (ConditionalCheckFailedException)
                        {
                            Console.WriteLine("ConditionalCheckFailedException. Retrying again...");
                        }
                    }
                }
            }
        }

        public List<T> GetTableQuery<T>(string column, string id, string indexName)
        {
            using (var client = new AmazonDynamoDBClient(_region))
            {
                using (var ctx = new DynamoDBContext(client))
                {
                    var row = ctx.FromQueryAsync<T>(new QueryOperationConfig
                    {
                        IndexName = indexName,
                        KeyExpression = new Expression
                        {
                            ExpressionStatement = "#key = :val",
                            ExpressionAttributeNames = new Dictionary<string, string> {
                                { "#key", column }
                            },
                            ExpressionAttributeValues = new Dictionary<string, DynamoDBEntry> {
                                { ":val",  id }
                            }
                        }
                    });

                    return row.GetRemainingAsync().Result;
                }
            }
        }

        public List<T> GetTableScan<T>(string column, string id)
        {
            using (var client = new AmazonDynamoDBClient(_region))
            {
                using (var ctx = new DynamoDBContext(client))
                {
                    var row = ctx.FromScanAsync<T>(new ScanOperationConfig
                    {
                        FilterExpression = new Expression
                        {
                            ExpressionStatement = "#key = :val",
                            ExpressionAttributeNames = new Dictionary<string, string> {
                                { "#key", column }
                            },
                            ExpressionAttributeValues = new Dictionary<string, DynamoDBEntry> {
                                { ":val",  id }
                            }
                        }
                    });

                    return row.GetRemainingAsync().Result;
                }
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
                            if (meta == null)
                            {
                                meta = new Meta
                                {
                                    Id = key,
                                    Key = set
                                };
                            }

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
            return await QueryDataAttributes(tableName, new AttributeValue { S = key }, "Id", null);
        }

        public async Task<Dictionary<string, AttributeValue>> QueryDataAttributes(string tableName, AttributeValue key, string field, string index)
        {
            using (var client = new AmazonDynamoDBClient(_region))
            {
                var resp = await client.QueryAsync(new QueryRequest
                {
                    TableName = tableName,
                    IndexName = index,
                    KeyConditionExpression = "#key = :v_Id",
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                        { ":v_Id", key }
                    },
                    ExpressionAttributeNames = new Dictionary<string, string>
                    {
                        { "#key", field }
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
    }
}
