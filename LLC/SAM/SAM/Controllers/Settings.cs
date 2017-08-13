using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Amazon.DynamoDBv2;

namespace SAM.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    public class SettingsController : DynamoDbController
    {
        // GET api/settings
        [HttpGet]
        public JsonResult Get()
        {
            using (var client = new AmazonDynamoDBClient(Region))
            {
                var results = Settings(client, "LLC-Settings");
                return Json(results);
            }
        }
    }
}
