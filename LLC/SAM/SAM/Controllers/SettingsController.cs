using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using SAM.DI;
using System.Linq;
using DbCore.Models;
using SAM.Models.EF;
using Newtonsoft.Json;
using System;
using SAM.Models.Auth;

namespace SAM.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    [CustomAuthorize(Access = "Admin")]
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
            if (setting.Delete && !string.IsNullOrEmpty(setting.Id))
            {
                return Json(_service.DeleteSetting(setting));
            }
            else if (ModelState.IsValid)
            {
                var user = new User(HttpContext.User.Claims);
                return Json(_service.SaveSetting(setting, user));
            }

            return Json(new Save { Status = false });
        }
    }
}
