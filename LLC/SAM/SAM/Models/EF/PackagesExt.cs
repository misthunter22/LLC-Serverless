using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;

namespace DbCore.Models
{
    public class PackagesExt
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string UploadedBy { get; set; }

        public DateTime? DateUploaded { get; set; }

        public string Key { get; set; }

        public bool? PackageProcessed { get; set; }

        public string FileName { get; set; }

        public string ImsSchema { get; set; }

        public string ImsSchemaVersion { get; set; }

        public string ImsTitle { get; set; }

        public string ImsDescription { get; set; }

        public bool Delete { get; set; }

        [BindNever]
        public IFormFile File { get; set; }

        public bool CanCreate()
        {
            return !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Description) &&
                !string.IsNullOrEmpty(FileName);
        }
    }
}
