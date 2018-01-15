using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using SAM.DI;
using SAM.Models.Auth;
using System;
using SAM.Models.Reports;

namespace SAM.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    [CustomAuthorize(Access = "Report")]
    public class ScreenshotsController : Controller
    {
        private ILLCData _service;

        public ScreenshotsController(ILLCData service)
        {
            _service = service;
        }

        // POST api/screenshots
        [HttpPost]
        public JsonResult Post([FromBody] BucketLocationsRequest m)
        {
            var model = _service.Screenshots(m);
            return Json(model);
        }
    }
}
