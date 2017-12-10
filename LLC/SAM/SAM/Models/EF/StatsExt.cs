using System;

namespace SAM.Models.EF
{
    public class StatsExt
    {
        public string Source { get; set; }

        public int Objects { get; set; }

        public int TotalLinks { get; set; }

        public int InvalidLinks { get; set; }

        public int HtmlFiles { get; set; }

        public DateTime? LastExtracted { get; set; }

        public DateTime? LastChecked { get; set; }
    }
}
