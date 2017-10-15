using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using SAM.DI;
using SAM.Models.Admin;

namespace SAM.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    public class DashboardController : Controller
    {
        private ILLCData _service;

        public DashboardController(ILLCData service)
        {
            _service = service;
        }

        // GET api/dashboard
        [HttpGet("{id}")]
        public JsonResult Get(string id)
        {
            var results = _service.Source("LLC-Sources", "LLC-Buckets", id, SourceSearchType.Id);
            return Json(results);
        }
    }
}
