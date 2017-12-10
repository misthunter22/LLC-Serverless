using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using SAM.DI;
using SAM.Models.Auth;

namespace SAM.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    [CustomAuthorize(Access = "Admin")]
    public class StatsController : Controller
    {
        private ILLCData _service;

        public StatsController(ILLCData service)
        {
            _service = service;
        }

        // GET api/sources
        [HttpGet]
        public JsonResult Get()
        {
            return Json(_service.Stats());
        }
    }
}
