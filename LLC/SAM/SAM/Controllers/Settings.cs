using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Amazon.DynamoDBv2;
using SAM.DI;

namespace SAM.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    public class SettingsController : Controller
    {
        private IDynamoDb _service;

        public SettingsController(IDynamoDb service)
        {
            _service = service;
        }

        // GET api/settings
        [HttpGet]
        public JsonResult Get()
        {
            using (var client = new AmazonDynamoDBClient(_service.Region()))
            {
                var results = _service.Settings(client, "LLC-Settings");
                return Json(results);
            }
        }
    }
}
