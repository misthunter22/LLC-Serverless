using System;
using System.Collections.Generic;

namespace DbCore.Models
{
    public partial class ObjectLinks
    {
        public string Id { get; set; }
        public string Object { get; set; }
        public string Link { get; set; }
        public DateTime? DateFirstFound { get; set; }
        public DateTime? DateLastFound { get; set; }
        public DateTime? DateRemoved { get; set; }
    }
}
