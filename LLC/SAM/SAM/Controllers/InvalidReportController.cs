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
        private ILLCData _service;

        public InvalidReportController(ILLCData service)
        {
            _service = service;
        }

        // GET api/invalidreport
        [HttpGet]
        public JsonResult Get()
        {
            using (var client = new AmazonDynamoDBClient(_service.Region()))
            {
                var results = _service.InvalidLinks(client, "LLC-Reports");
                return Json(results);
            }
        }
    }
}
