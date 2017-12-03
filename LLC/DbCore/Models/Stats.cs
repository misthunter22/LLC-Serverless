using System;
using System.Collections.Generic;

namespace DbCore.Models
{
    public partial class Stats
    {
        public string Id { get; set; }
        public string Link { get; set; }
        public long? ContentSize { get; set; }
        public int? DownloadTime { get; set; }
        public DateTime? DateChecked { get; set; }
        public string Error { get; set; }
        public string StatusCode { get; set; }
        public string StatusDescription { get; set; }
        public string ContentType { get; set; }
    }
}
