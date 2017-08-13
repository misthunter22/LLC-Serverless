using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Amazon.DynamoDBv2;

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
                var results = Source(client, "LLC-Sources", "LLC-Buckets", id);
                return Json(results);
            }
        }
    }
}
