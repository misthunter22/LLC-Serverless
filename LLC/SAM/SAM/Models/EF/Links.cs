using System;
using System.Collections.Generic;

namespace DbCore.Models
{
    public partial class Links
    {
        public string Id { get; set; }
        public string Source { get; set; }
        public string Url { get; set; }
        public DateTime? DateFirstFound { get; set; }
        public DateTime? DateLastFound { get; set; }
        public DateTime? DisabledDate { get; set; }
        public string DisabledUser { get; set; }
        public int? AttemptCount { get; set; }
        public bool? Valid { get; set; }
        public DateTime? DateLastChecked { get; set; }
        public int? AllTimeMinDownloadTime { get; set; }
        public int? AllTimeMaxDownloadTime { get; set; }
        public decimal? AllTimeStdDevDownloadTime { get; set; }
        public int? PastWeekMinDownloadTime { get; set; }
        public int? PastWeekMaxDownloadTime { get; set; }
        public decimal? PastWeekStdDevDownloadTime { get; set; }
        public DateTime? DateUpdated { get; set; }
        public DateTime? ReportNotBeforeDate { get; set; }
    }
}
