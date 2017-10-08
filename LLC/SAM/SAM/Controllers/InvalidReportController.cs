using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using SAM.DI;
using System.Linq.Dynamic.Core;
using SAM.Models.Reports;
using System.Collections.Generic;
using System.Linq;

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
            var results = _service.InvalidLinks("LLC-Reports");
            List<InvalidLinksModel> filter;

            // Do the sorting first
            if (m.direction == "asc")
                filter = results.AsQueryable().OrderBy(m.columnName + " ascending").ToList();
            else
                filter = results.AsQueryable().OrderBy(m.columnName + " descending").ToList();

            // Look for any that match the string
            if (!string.IsNullOrEmpty(m.search))
            {
                filter = filter.Where(x => x.AttemptCount.ToString().Contains(m.search) ||
                                            (x.DateLastChecked == null ? false : x.DateLastChecked.ToString().Contains(m.search)) ||
                                            (x.DateLastFound == null ? false : x.DateLastFound.ToString().Contains(m.search)) ||
                                            x.Id.ToString().Contains(m.search) ||
                                            x.Link.ToString().Contains(m.search) ||
                                            (x.Source == null ? false : x.Source.Contains(m.search)) ||
                                            (x.Url == null ? false : x.Url.Contains(m.search))).ToList();
            }

            var filterCount = filter.Count;
            filter = filter.Skip(m.start).Take(m.length).ToList();

            var model   = new DataTableModel<InvalidLinksModel>
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
