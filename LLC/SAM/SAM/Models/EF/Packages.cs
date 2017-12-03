using System;
using System.Collections.Generic;

namespace DbCore.Models
{
    public partial class Packages
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
    }
}
