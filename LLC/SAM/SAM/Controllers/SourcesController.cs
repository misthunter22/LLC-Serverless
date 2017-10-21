using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using SAM.DI;

namespace SAM.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    public class SourcesController : Controller
    {
        private ILLCData _service;

        public SourcesController(ILLCData service)
        {
            _service = service;
        }

        // GET api/sources
        [HttpGet]
        public JsonResult Get()
        {
            var results = _service.Sources();
            return Json(results);
        }
    }
}
