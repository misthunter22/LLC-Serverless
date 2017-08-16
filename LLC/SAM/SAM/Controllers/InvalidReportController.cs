using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Amazon.DynamoDBv2;
using SAM.DI;

namespace SAM.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    public class InvalidReportController : Controller
    {
        private IDynamoDb _service;

        public InvalidReportController(IDynamoDb service)
        {
            _service = service;
        }

        // GET api/sources
        [HttpGet]
        public JsonResult Get()
        {
            using (var client = new AmazonDynamoDBClient(_service.Region()))
            {
                var results = _service.Sources(client, "LLC-Sources", "LLC-Buckets");
                return Json(results);
            }
        }
    }
}
