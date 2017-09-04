using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Amazon.DynamoDBv2;
using SAM.DI;
using System.Linq.Dynamic.Core;
using SAM.Models.Reports;
using System.Collections.Generic;
using System.Linq;

namespace SAM.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    public class BucketLocationsController : Controller
    {
        private ILLCData _service;

        public BucketLocationsController(ILLCData service)
        {
            _service = service;
        }

        // POST api/invalidreport
        [HttpPost]
        public JsonResult Post([FromBody] BucketLocationsRequest m)
        {
            using (var client = new AmazonDynamoDBClient(_service.Region()))
            {
                var model = _service.BucketLocations(client, m.id, "LLC-ObjectLinks", "LLC-Objects","LLC-Buckets");
                return Json(model);
            }
        }
    }
}
