using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using SAM.DI;
using SAM.Models.Reports;

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
            var model = _service.BucketLocations(m, "LLC-ObjectLinks", "LLC-Objects", "LLC-Buckets", "LLC-Stats");
            return Json(model);
        }
    }
}
