using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Amazon.DynamoDBv2;
using Amazon;
using SAM.Models.Admin;
using System;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;
using System.Collections.Generic;

namespace SAM.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    public class AdminController : Controller
    {
        // GET api/admin
        [HttpGet("{id}")]
        public JsonResult Get(string id)
        {
            using (var client = new AmazonDynamoDBClient(RegionEndpoint.USWest2))
            {
                var dash = new DashboardModel();

                switch (id)
                {
                    case "object":
                        dash.Data = TableCount(client, "LLC-Objects");
                        break;
                    case "links":
                        dash.Data = TableCount(client, "LLC-Links");
                        break;
                    case "invalid":
                        dash.Data = QueryCountBool(client, "LLC-Links", "Valid", true).Result;
                        break;
                    case "html":
                        dash.Data = QueryCountContains(client, "LLC-Objects", "ItemName", ".htm").Result;
                        break;
                    case "extracted":
                        Console.WriteLine("Case 2");
                        break;
                    case "checked":
                        Console.WriteLine("Case 2");
                        break;
                    default:
                        break;
                }

                return Json(dash);
            }
        }

        private string TableCount(AmazonDynamoDBClient client, string tableName)
        {
            var descr = client.DescribeTableAsync(tableName);
            var count = descr.Result.Table.ItemCount;
            return count.ToString();
        }

        private async Task<string> QueryCountBool(AmazonDynamoDBClient client, string tableName, string column, bool b)
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

        private async Task<string> QueryCountContains(AmazonDynamoDBClient client, string tableName, string column, string s)
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
