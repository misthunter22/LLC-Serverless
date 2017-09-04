using System;
using System.Reflection;

namespace SAM.Models.Reports
{
    public class InvalidLinksModel
    {
        public int Id { get; set; }

        public int Link { get; set; }

        public int AttemptCount { get; set; }

        public string DateLastFound { get; set; }

        public string Source { get; set; }

        public string Url { get; set; }

        public string DateLastChecked { get; set; }
    }
}
