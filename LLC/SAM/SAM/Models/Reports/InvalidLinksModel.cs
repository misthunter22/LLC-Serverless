using System;

namespace SAM.Models.Reports
{
    public class InvalidLinksModel
    {
        public int Id { get; set; }

        public int Link { get; set; }

        public int AttemptCount { get; set; }

        public DateTime? DateLastFound { get; set; }

        public string Source { get; set; }

        public string Url { get; set; }

        public DateTime? DateLastChecked { get; set; }
    }
}
