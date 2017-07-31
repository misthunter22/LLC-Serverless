using Microsoft.AspNetCore.Mvc;
using Amazon.DynamoDBv2;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;
using System.Collections.Generic;
using System.Linq;
using Amazon;
using SAM.Models.Admin;

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

        protected async Task<List<DashboardModel>> Sources(AmazonDynamoDBClient client, string tableName)
        {
            var sources = await client.ScanAsync(new ScanRequest
            {
                TableName = tableName
            });

            var array = new List<DashboardModel>();
            foreach (var d in sources.Items)
            {
                int i = int.Parse(d["Id"].N);
                if (i > 0)
                {
                    array.Add(new DashboardModel
                    {
                        Source = i.ToString(),
                        Title = d["Name"].S
                    });
                }
            }

            return array;
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
    }
}
