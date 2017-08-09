using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Amazon.DynamoDBv2;

namespace SAM.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    public class SourcesController : DynamoDbController
    {
        // GET api/sources
        [HttpGet]
        public JsonResult Get()
        {
            using (var client = new AmazonDynamoDBClient(Region))
            {
                var sources = Sources(client, "LLC-Sources", "LLC-Buckets");
                return Json(sources);
            }
        }
    }
}
