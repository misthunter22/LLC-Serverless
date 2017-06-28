using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Amazon.DynamoDBv2;
using Amazon;
using SAM.Models.Admin;
using System;

namespace SAM.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    public class AdminController : Controller
    {
        // GET api/admin
        [HttpGet]
        public JsonResult Get()
        {
            using (var client = new AmazonDynamoDBClient(RegionEndpoint.USWest2))
            {
                var count = new DashboardModel()
                {
                    HtmlFiles = 0,
                    InvalidLinks = 0,
                    LastExtracted = DateTime.Now,
                    LastUpdated = DateTime.Now,
                    Links = TableCount(client, "LLC-Links"),
                    Objects = TableCount(client, "LLC-Objects")
                };

                return Json(count);
            }
        }

        private long TableCount(AmazonDynamoDBClient client, string tableName)
        {
            var descr = client.DescribeTableAsync(tableName);
            var count = descr.Result.Table.ItemCount;
            return count;
        }
    }
}
