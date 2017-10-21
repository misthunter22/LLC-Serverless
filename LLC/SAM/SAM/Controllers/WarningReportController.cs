using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using SAM.DI;
using System.Linq.Dynamic.Core;
using SAM.Models.Reports;
using System.Collections.Generic;
using System.Linq;
using SAM.Models.Dynamo;

namespace SAM.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    public class WarningReportController : Controller
    {
        private ILLCData _service;

        public WarningReportController(ILLCData service)
        {
            _service = service;
        }

        // POST api/invalidreport
        [HttpPost]
        public JsonResult Post([FromBody] DataTableRequest m)
        {
            var results = _service.WarningLinks();
            List<WarningLinks> filter;

            // Do the sorting first
            if (m.direction == "asc")
                filter = results.AsQueryable().OrderBy(m.columnName + " ascending").ToList();
            else
                filter = results.AsQueryable().OrderBy(m.columnName + " descending").ToList();

            // Look for any that match the string
            if (!string.IsNullOrEmpty(m.search))
            {
                filter = filter.Where(x => x.ContentSize.ToString().Contains(m.search) ||
                                            (x.Id.ToString().Contains(m.search)) ||
                                            (x.LinkId.ToString().Contains(m.search)) ||
                                            x.Mean.ToString().Contains(m.search) ||
                                            x.SdRange.ToString().Contains(m.search) ||
                                            (x.StandardDeviation.ToString().Contains(m.search)) ||
                                            (x.StatId.ToString().Contains(m.search))).ToList();
            }

            var filterCount = filter.Count;
            filter = filter.Skip(m.start).Take(m.length).ToList();
            _service.AddUrlToWarningLinks(filter);

            var model   = new DataTableModel<WarningLinks>
            {
                data = filter,
                draw = m.draw,
                recordsFiltered = filterCount,
                recordsTotal = filter.Count < m.length ? filter.Count : m.length
            };

            return Json(model);
        }
    }
}
