using Microsoft.AspNetCore.Mvc;
using Amazon.DynamoDBv2;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;
using System.Collections.Generic;
using System.Linq;
using Amazon;
using SAM.Models.Admin;
using System;

namespace SAM.Controllers
{
    public abstract class DynamoDbController : Controller
    {
        protected RegionEndpoint Region = RegionEndpoint.USWest2;

        protected string TableCount(AmazonDynamoDBClient client, string tableName)
        {
            var descr = client.DescribeTableAsync(tableName);
            var count = descr.Result.Table.ItemCount;
            return count.ToString();
        }

        protected async Task<List<SourceModel>> Sources(AmazonDynamoDBClient client, string tableName)
        {
            var sources = await client.ScanAsync(new ScanRequest
            {
                TableName = tableName
            });

            var array = new List<SourceModel>();
            foreach (var d in sources.Items)
            {
                int i = int.Parse(d["Id"].N);
                array.Add(new SourceModel
                {
                    AllowLinkChecking    = d.ContainsKey("AllowLinkChecking")    ? d["AllowLinkChecking"].BOOL         : (bool?)null,
                    AllowLinkExtractions = d.ContainsKey("AllowLinkExtractions") ? d["AllowLinkExtractions"].BOOL      : (bool?)null,
                    DateCreated          = d.ContainsKey("DateCreated")          ? ParseDate(d["DateCreated"].S)       : null,
                    DateLastChecked      = d.ContainsKey("DateLastChecked")      ? ParseDate(d["DateLastChecked"].S)   : null,
                    DateLastExtracted    = d.ContainsKey("DateLastExtracted")    ? ParseDate(d["DateLastExtracted"].S) : null,
                    Description          = d.ContainsKey("Description")          ? d["Description"].S                  : null,
                    HtmlFileCount        = d.ContainsKey("HtmlFileCount")        ? ParseInt(d["HtmlFileCount"].N)      : -1,
                    InvalidLinkCount     = d.ContainsKey("InvalidLinkCount")     ? ParseInt(d["InvalidLinkCount"].N)   : -1,
                    LinkCount            = d.ContainsKey("LinkCount")            ? ParseInt(d["LinkCount"].N)          : -1,
                    S3BucketId           = d.ContainsKey("S3BucketId")           ? ParseInt(d["S3BucketId"].N)         : -1,
                    S3ObjectCount        = d.ContainsKey("S3ObjectCount")        ? ParseInt(d["S3ObjectCount"].N)      : -1,
                    Source               = i,
                    Title                = d.ContainsKey("Name") ? d["Name"].S : null,
                });
            }

            return array;
        }

        protected async Task<SourceModel> Source(AmazonDynamoDBClient client, string tableName, string id)
        {
            var sources = await Sources(client, tableName);
            return sources.FirstOrDefault(x => x.Source.Equals(id));
        }

        protected async Task<string> QueryCountBool(AmazonDynamoDBClient client, string tableName, string column, bool b)
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

        protected async Task<string> QueryDataInt(AmazonDynamoDBClient client, string tableName, string key, string field)
        {
            var resp = await client.QueryAsync(new QueryRequest
            {
                TableName = tableName,
                KeyConditionExpression = "Id = :v_Id",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                    {":v_Id", new AttributeValue { N =  key }}}
            });

            var dict = resp.Items.FirstOrDefault();
            var ret  = string.Empty;
            if (dict != null)
            {
                ret = dict[field].S;
            }

            return ret;
        }

        protected async Task<string> QueryCountContains(AmazonDynamoDBClient client, string tableName, string column, string s)
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

        protected DateTime? ParseDate(string date)
        {
            DateTime ret = DateTime.MinValue;
            if (DateTime.TryParse(date, out ret))
            {
                return ret;
            }

            return null;
        }

        protected int ParseInt(string i)
        {
            int ret = -1;
            int.TryParse(i, out ret);
            return ret;
        }
    }
}
