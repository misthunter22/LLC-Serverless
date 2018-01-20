using System;
using System.Collections.Generic;

namespace DbCore.Models
{
    public partial class PackageFiles
    {
        public string Id { get; set; }
        public string CourseLocation { get; set; }
        public string Link { get; set; }
        public string Protocol { get; set; }
        public string LinkName { get; set; }
        public string ParentFolder { get; set; }
        public string PackageId { get; set; }
    }
}
