using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using SAM.DI;
using System.Linq;
using DbCore.Models;
using SAM.Models.EF;

namespace SAM.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    public class SourcesController : Controller
    {
        private ILLCData _service;

        public SourcesController(ILLCData service)
        {
            _service = service;
        }

        // GET api/sources
        [HttpGet]
        public JsonResult Get()
        {
            return Json(_service.Sources());
        }

        // GET api/sources/{id}
        [HttpGet("{id}")]
        public JsonResult GetId(string id)
        {
            var source = _service.Source(id, Models.Admin.SearchType.Id);
            var bucket = _service.Buckets().FirstOrDefault(x => x.Id == source.S3bucketId);
            source.S3bucketName         = bucket.Name;
            source.S3bucketSearchPrefix = bucket.SearchPrefix;
            source.Region               = bucket.Region;
            source.SecretKey            = bucket.SecretKey;
            source.AccessKey            = bucket.AccessKey;

            return Json(source);
        }

        [HttpPost]
        public JsonResult Post([FromBody] SourcesExt source)
        {
            if (ModelState.IsValid)
            {
                if (source.Delete)
                    return Json(_service.DeleteSource(source));
                else
                    return Json(_service.SaveSource(source));
            }

            return Json(new Save { Status = false });
        }
    }
}
