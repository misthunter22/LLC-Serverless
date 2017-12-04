using DbCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbCore.Models
{
    public class ReportsExt
    {
        public string Id { get; set; }
        public long? ContentSize { get; set; }
        public long? Mean { get; set; }
        public double? StandardDeviation { get; set; }
        public int? SdMaximum { get; set; }
        public string Link { get; set; }
        public string Stat { get; set; }
        public string ReportType { get; set; }

        public int? AttemptCount { get; set; }

        public DateTime? DateLastChecked { get; set; }

        public DateTime? DateLastFound { get; set; }

        public string Source { get; set; }

        public string Url { get; set; }
    }
}
