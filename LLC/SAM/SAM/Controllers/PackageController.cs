using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using SAM.DI;
using SAM.Models.Auth;

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

        // GET api/settings
        [HttpGet]
        
        public JsonResult Get()
        {
            var results = _service.Setting("PackageBucket", Models.Admin.SearchType.Name);
            return Json(results);
        }
    }
}
