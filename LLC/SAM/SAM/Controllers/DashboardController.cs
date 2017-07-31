using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Amazon.DynamoDBv2;
using SAM.Models.Admin;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SAM.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    public class DashboardController : DynamoDbController
    {
        // GET api/dashboard
        [HttpGet("{id}")]
        public JsonResult Get(string id)
        {
            using (var client = new AmazonDynamoDBClient(Region))
            {
                var dash = new DashboardModel { Source = id, Data = new List<DashboardData>() };

                var task1 = Task.Factory.StartNew(() => 
                    dash.Data.Add(new DashboardData { Key = "object", Data = TableCount(client, "LLC-Objects") })); // Needs updated based on source ID
                var task2 = Task.Factory.StartNew(() => 
                    dash.Data.Add(new DashboardData { Key = "links", Data = TableCount(client, "LLC-Links") }));    // Needs updated based on source ID
                var task3 = Task.Factory.StartNew(() => 
                    dash.Data.Add(new DashboardData { Key = "invalid", Data = QueryCountBool(client, "LLC-Links", "Valid", true).Result }));
                var task4 = Task.Factory.StartNew(() =>
                    dash.Data.Add(new DashboardData { Key = "html", Data = QueryCountContains(client, "LLC-Objects", "ItemName", ".htm").Result }));
                var task5 = Task.Factory.StartNew(() =>
                    dash.Data.Add(new DashboardData { Key = "extracted", Data = QueryDataInt(client, "LLC-Sources", id, "DateLastExtracted").Result }));
                var task6 = Task.Factory.StartNew(() =>
                    dash.Data.Add(new DashboardData { Key = "checked", Data = QueryDataInt(client, "LLC-Sources", id, "DateLastChecked").Result }));

                Task.WaitAll(task1, task2, task3, task4, task5, task6);

                return Json(dash);
            }
        }
    }
}
