using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Amazon.DynamoDBv2;
using SAM.DI;
using System.Linq;
using SAM.Models.Reports;
using System.Collections.Generic;

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
                List<InvalidLinksModel> filter;
                if (m.direction == "asc")
                {
                    filter = results.OrderBy(x => x.ElementAt(m.columnName, m.column)).ToList();
                    filter = filter.Skip(m.start).Take(m.length).ToList();
                }
                else
                {
                    filter = results.OrderByDescending(x => x.ElementAt(m.columnName, m.column)).ToList();
                    filter = filter.Skip(m.start).Take(m.length).ToList();
                }
                
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
