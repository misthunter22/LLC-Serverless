using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using SAM.DI;

namespace SAM.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    public class ReportController : Controller
    {
        private ILLCData _service;

        public ReportController(ILLCData service)
        {
            _service = service;
        }

        // GET api/report
        [HttpGet]
        public ActionResult Get(string id)
        {
            var url = _service.Link(id);
            return Redirect(url.Url);
        }
    }
}
