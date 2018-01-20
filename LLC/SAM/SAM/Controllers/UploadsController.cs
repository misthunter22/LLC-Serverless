using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using SAM.DI;
using DbCore.Models;
using SAM.Models.EF;
using SAM.Models.Auth;
using Microsoft.AspNetCore.Http;

namespace SAM.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    [CustomAuthorize(Access = "Admin")]
    public class UploadsController : Controller
    {
        private ILLCData _service;

        public UploadsController(ILLCData service)
        {
            _service = service;
        }

        // GET api/settings
        [HttpGet]
        
        public JsonResult Get()
        {
            var results = _service.Packages();
            return Json(results);
        }

        // GET api/sources/{id}
        [HttpGet("{id}")]
        public JsonResult GetId(string id)
        {
            var setting = _service.Package(id);
            return Json(setting);
        }

        [HttpPost]
        public JsonResult Post([FromBody] PackagesExt model)
        {
            if (model.Delete && !string.IsNullOrEmpty(model.Id))
            {
                return Json(_service.DeletePackage(model));
            }
            else if (ModelState.IsValid)
            {
                var user = new User(HttpContext.User.Claims);
                //return Json(_service.SavePackage(model));
                return null;
            }

            return Json(new Save { Status = false });
        }
    }
}
