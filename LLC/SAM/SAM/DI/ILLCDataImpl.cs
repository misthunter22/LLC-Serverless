using Amazon.DynamoDBv2;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;
using System.Collections.Generic;
using System.Linq;
using Amazon;
using SAM.Models.Admin;
using System;
using SAM.Models.Reports;

namespace SAM.DI
{
    public class ILLCDataImpl : ILLCData
    {
        protected RegionEndpoint _region = RegionEndpoint.USWest2;

        public RegionEndpoint Region()
        {
            return _region;
        }

        public string TableCount(AmazonDynamoDBClient client, string tableName)
        {
            var descr = client.DescribeTableAsync(tableName);
            var count = descr.Result.Table.ItemCount;
            return count.ToString();
        }

        public List<SourceModel> Sources(AmazonDynamoDBClient client, string tableName, string bucketTableName)
        {
            var results = client.ScanAsync(new ScanRequest
            {
                TableName = tableName
            }).Result;

            var array = new List<SourceModel>();
            foreach (var d in results.Items)
            {
                int i = int.Parse(d["Id"].N);
                var m = new SourceModel
                {
                    AllowLinkChecking = d.ContainsKey("AllowLinkChecking") ? d["AllowLinkChecking"].BOOL : false,
                    AllowLinkExtractions = d.ContainsKey("AllowLinkExtractions") ? d["AllowLinkExtractions"].BOOL : false,
                    DateCreated = d.ContainsKey("DateCreated") ? ParseDate(d["DateCreated"].S) : null,
                    DateLastChecked = d.ContainsKey("DateLastChecked") ? ParseDate(d["DateLastChecked"].S) : null,
                    DateLastExtracted = d.ContainsKey("DateLastExtracted") ? ParseDate(d["DateLastExtracted"].S) : null,
                    Description = d.ContainsKey("Description") ? d["Description"].S : null,
                    HtmlFileCount = d.ContainsKey("HtmlFileCount") ? ParseInt(d["HtmlFileCount"].N) : -1,
                    InvalidLinkCount = d.ContainsKey("InvalidLinkCount") ? ParseInt(d["InvalidLinkCount"].N) : -1,
                    LinkCount = d.ContainsKey("LinkCount") ? ParseInt(d["LinkCount"].N) : -1,
                    S3BucketId = d.ContainsKey("S3BucketId") ? ParseInt(d["S3BucketId"].N) : -1,
                    S3ObjectCount = d.ContainsKey("S3ObjectCount") ? ParseInt(d["S3ObjectCount"].N) : -1,
                    Source = i,
                    Title = d.ContainsKey("Name") ? d["Name"].S : null,
                };

                m.S3ObjectName = QueryDataInt(client, bucketTableName, m.S3BucketId.ToString(), "Name").Result;
                array.Add(m);
            }

            return array.OrderBy(x => x.Source).ToList();
        }

        public SourceModel Source(AmazonDynamoDBClient client, string tableName, string bucketTableName, string id)
        {
            var results = Sources(client, tableName, bucketTableName);
            return results.FirstOrDefault(x => x.Source.Equals(id));
        }

        public List<SettingModel> Settings(AmazonDynamoDBClient client, string tableName)
        {
            var results = client.ScanAsync(new ScanRequest
            {
                TableName = tableName
            }).Result;

            var array = new List<SettingModel>();
            foreach (var d in results.Items)
            {
                int i = int.Parse(d["Id"].N);
                var m = new SettingModel
                {
                    DateCreated = d.ContainsKey("DateCreated") ? ParseDate(d["DateCreated"].S) : null,
                    DateModified = d.ContainsKey("DateModified") ? ParseDate(d["DateModified"].S) : null,
                    Description = d.ContainsKey("Description") ? d["Description"].S : null,
                    Id = i,
                    ModifiedUser = d.ContainsKey("ModifiedUser") ? d["ModifiedUser"].S : null,
                    Name = d.ContainsKey("Name") ? d["Name"].S : null,
                    Value = d.ContainsKey("Value") ? d["Value"].S : null
                };

                array.Add(m);
            }

            return array.OrderBy(x => x.Name).ToList();
        }

        public List<InvalidLinksModel> InvalidLinks(AmazonDynamoDBClient client, string tableName)
        {
            var rows = client.ScanAsync(new ScanRequest
            {
                ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                    { ":val",  new AttributeValue { S = "Invalid" } }
                },
                FilterExpression = "ReportType = :val",
                TableName = tableName
            }).Result;

            Console.WriteLine($"Invalid Report Item Count: {rows.Items.Count}");

            return rows.Items.Select(x => new InvalidLinksModel
            {
                AttemptCount    = x.ContainsKey("AttemptCount")    ? ParseInt(x["AttemptCount"].N)     : -1,
                DateLastChecked = x.ContainsKey("DateLastChecked") ? ParseDate(x["DateLastChecked"].N) : null,
                DateLastFound   = x.ContainsKey("DateLastFound")   ? ParseDate(x["DateLastFound"].N)   : null,
                Id              = ParseInt(x["Id"].N),
                Link            = ParseInt(x["Link"].N),
                Source          = x["Source"].S,
                Url             = x["Url"].S
            })
            .ToList();
        }

        public async Task<string> QueryCountBool(AmazonDynamoDBClient client, string tableName, string column, bool b)
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

        public async Task<string> QueryDataInt(AmazonDynamoDBClient client, string tableName, string key, string field)
        {
            var resp = await client.QueryAsync(new QueryRequest
            {
                TableName = tableName,
                KeyConditionExpression = "Id = :v_Id",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                    {":v_Id", new AttributeValue { N =  key }}}
            });

            var dict = resp.Items.FirstOrDefault();
            var ret = string.Empty;
            if (dict != null)
            {
                ret = dict[field].S;
            }

            return ret;
        }

        public async Task<string> QueryCountContains(AmazonDynamoDBClient client, string tableName, string column, string s)
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

        private DateTime? ParseDate(string date)
        {
            DateTime ret = DateTime.MinValue;
            if (DateTime.TryParse(date, out ret))
            {
                return ret;
            }

            return null;
        }

        private int ParseInt(string i)
        {
            int ret = -1;
            int.TryParse(i, out ret);
            return ret;
        }
    }
}
