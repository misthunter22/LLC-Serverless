using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Amazon.DynamoDBv2;
using SAM.DI;

namespace SAM.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    public class SourcesController : Controller
    {
        private ILLCData _service;

        public SourcesController(ILLCData service)
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
