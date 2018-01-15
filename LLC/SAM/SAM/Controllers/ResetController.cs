using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using SAM.DI;
using SAM.Models.Auth;

namespace SAM.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    [CustomAuthorize(Access = "Admin")]
    public class ResetController : Controller
    {
        private ILLCData _service;

        public ResetController(ILLCData service)
        {
            _service = service;
        }

        // GET api/reset
        [HttpGet("{id}")]
        public JsonResult Get(string id)
        {
            return Json(_service.Reset(id));
        }
    }
}
