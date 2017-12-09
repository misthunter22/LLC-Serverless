using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using SAM.DI;
using System.Linq;
using DbCore.Models;
using SAM.Models.EF;

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

        // GET api/sources/{id}
        [HttpGet("{id}")]
        public JsonResult GetId(string id)
        {
            var setting = _service.Settings().FirstOrDefault(x => x.Id == id);
            return Json(setting);
        }

        [HttpPost]
        public JsonResult Post([FromBody] SettingsExt setting)
        {
            if (ModelState.IsValid)
            {
                if (setting.Delete)
                    return Json(_service.DeleteSetting(setting));
                else
                    return Json(_service.SaveSetting(setting));
            }

            return Json(new Save { Status = false });
        }
    }
}
