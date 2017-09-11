using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Amazon.DynamoDBv2;
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
        public ActionResult Get(int id)
        {
            using (var client = new AmazonDynamoDBClient(_service.Region()))
            {
                var url = _service.QueryDataAttribute(client, "LLC-Links", id.ToString(), "Url");
                return Redirect(url.Result.S);
            }
        }
    }
}
