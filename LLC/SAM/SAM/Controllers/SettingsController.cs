using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using SAM.DI;

namespace SAM.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    public class SettingsController : Controller
    {
        private ILLCData _service;

        public SettingsController(ILLCData service)
        {
            _service = service;
        }

        // GET api/settings
        [HttpGet]
        public JsonResult Get()
        {
            var results = _service.Settings();
            return Json(results);
        }
    }
}
