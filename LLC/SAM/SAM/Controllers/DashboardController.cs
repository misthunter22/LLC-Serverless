using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Amazon.DynamoDBv2;
using SAM.DI;

namespace SAM.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    public class DashboardController : Controller
    {
        private ILLCData _service;

        public DashboardController(ILLCData service)
        {
            _service = service;
        }

        // GET api/dashboard
        [HttpGet("{id}")]
        public JsonResult Get(string id)
        {
            using (var client = new AmazonDynamoDBClient(_service.Region()))
            {
                var results = _service.Source(client, "LLC-Sources", "LLC-Buckets", id);
                return Json(results);
            }
        }
    }
}
