using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using SAM.DI;
using System.Linq.Dynamic.Core;
using SAM.Models.Reports;
using System.Collections.Generic;
using System.Linq;
using DbCore.Models;

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
            var results = _service.InvalidLinks();
            List<Reports> filter;

            // Do the sorting first
            if (m.direction == "asc")
                filter = results.AsQueryable().OrderBy(m.columnName + " ascending").ToList();
            else
                filter = results.AsQueryable().OrderBy(m.columnName + " descending").ToList();

            // Look for any that match the string
            if (!string.IsNullOrEmpty(m.search))
            {
                filter = filter.Where(x => x.Obj.AttemptCount.ToString().Contains(m.search) ||
                                            (x.Obj.DateLastChecked == null ? false : x.Obj.DateLastChecked.ToString().Contains(m.search)) ||
                                            (x.Obj.DateLastFound == null ? false : x.Obj.DateLastFound.ToString().Contains(m.search)) ||
                                            x.Id.ToString().Contains(m.search) ||
                                            x.Link.ToString().Contains(m.search) ||
                                            (x.Obj.Source == null ? false : x.Obj.Source.Contains(m.search)) ||
                                            (x.Obj.Url == null ? false : x.Obj.Url.Contains(m.search))).ToList();
            }

            var filterCount = filter.Count;
            filter = filter.Skip(m.start).Take(m.length).ToList();

            var model   = new DataTableModel<Reports>
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
