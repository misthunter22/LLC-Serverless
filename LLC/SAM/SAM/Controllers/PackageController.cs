using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using SAM.DI;
using SAM.Models.Auth;
using SAM.Models.Admin;
using System;
using Newtonsoft.Json;

namespace SAM.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    [CustomAuthorize(Access = "Admin")]
    public class PackageController : Controller
    {
        private ILLCData _service;

        public PackageController(ILLCData service)
        {
            _service = service;
        }

        // GET api/package
        [HttpGet]
        
        public JsonResult Get()
        {
            var results = _service.Setting("PackageBucket", Models.Admin.SearchType.Name);
            return Json(results);
        }

        // POST api/package
        [HttpPost]

        public JsonResult Post([FromBody] IdRequest model)
        {
            Console.WriteLine(JsonConvert.SerializeObject(model));

            var results = _service.PackageFiles(model.Id);
            return Json(results);
        }
    }
}
