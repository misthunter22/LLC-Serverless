using DbCore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
    }
}
