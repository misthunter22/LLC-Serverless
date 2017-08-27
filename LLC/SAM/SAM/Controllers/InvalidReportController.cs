using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Amazon.DynamoDBv2;
using SAM.DI;
using System.Linq;
using SAM.Models.Reports;

namespace SAM.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    public class InvalidReportController : Controller
    {
        private ILLCData _service;

        public InvalidReportController(ILLCData service)
        {
            _service = service;
        }

        // POST api/invalidreport
        [HttpPost]
        public JsonResult Post([FromBody] DataTableRequest m)
        {
            using (var client = new AmazonDynamoDBClient(_service.Region()))
            {
                var results = _service.InvalidLinks(client, "LLC-Reports");
                var filter  = results.Skip(m.start).Take(m.length).ToList();
                var model   = new DataTableModel<InvalidLinksModel>
                {
                    data = filter,
                    draw = m.draw,
                    recordsFiltered = results.Count,
                    recordsTotal = m.length
                };

                return Json(model);
            }
        }
    }
}
