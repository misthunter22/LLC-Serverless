using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using SAM.DI;
using DbCore.Models;
using SAM.Models.EF;
using SAM.Models.Auth;
using System;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.IO;

namespace SAM.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    [CustomAuthorize(Access = "Admin")]
    public class UploadsController : Controller
    {
        private ILLCData _service;

        public UploadsController(ILLCData service)
        {
            _service = service;
        }

        // GET api/settings
        [HttpGet]
        
        public JsonResult Get()
        {
            var results = _service.Packages();
            return Json(results);
        }

        // GET api/sources/{id}
        [HttpGet("{id}")]
        public JsonResult GetId(string id)
        {
            var setting = _service.Package(id);
            return Json(setting);
        }

        [HttpPost]
        public JsonResult Post(IFormCollection form)
        {
            var model = Map(form);

            if (model.Delete && !string.IsNullOrEmpty(model.Id))
            {
                return Json(_service.DeletePackage(model));
            }
            else if (ModelState.IsValid)
            {
                var user = new User(HttpContext.User.Claims);
                return Json(_service.SavePackage(model, user));
            }

            return Json(new Save { Status = false });
        }

        private PackagesExt Map(IFormCollection form)
        {
            var package = new PackagesExt();

            var name        = "name";
            var fileName    = "fileName";
            var description = "description";
            var delete      = "delete";
            var id          = "id";

            if (form.Any())
            {
                if (form.Keys.Contains(name))
                    package.Name = form[name];

                if (form.Keys.Contains(fileName))
                    package.FileName = form[fileName];

                if (form.Keys.Contains(description))
                    package.Description = form[description];

                if (form.Keys.Contains(delete))
                    package.Delete = bool.Parse(form[delete]);

                if (form.Keys.Contains(id))
                    package.Id = form[id];
            }

            // Output all w/o file
            Console.WriteLine(JsonConvert.SerializeObject(package));

            if (form.Files.Count == 1)
            {
                Console.WriteLine($"Found a package file with name {form.Files[0].Name}!");

                var fid = Guid.NewGuid().ToString();

                package.File = form.Files[0];
                package.Id   = fid;
                package.Key  = "Packages/" + fid;
                UploadFile(form.Files[0], package.Key);
            }

            return package;
        }

        private void UploadFile(IFormFile file, string key)
        {
            if (file == null || file.Length == 0)
                throw new Exception("File is empty!");

            using (var stream = file.OpenReadStream())
            using (var memoryStream = new MemoryStream())
            {
                var bucket = Environment.GetEnvironmentVariable("AppS3Bucket");
                stream.CopyTo(memoryStream);
                _service.FilePut(bucket, key, memoryStream);
            }
        }
    }
}
