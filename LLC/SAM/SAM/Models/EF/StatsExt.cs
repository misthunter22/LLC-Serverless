using System;
using System.ComponentModel.DataAnnotations;

namespace SAM.Models.EF
{
    public class StatsExt
    {
        public string Source { get; set; }

        public int Objects { get; set; }

        public int TotalLinks { get; set; }

        public int InvalidLinks { get; set; }

        public int HtmlFiles { get; set; }

        [DisplayFormat(DataFormatString = "yyyy-MM-dd")]
        public DateTime? LastExtracted { get; set; }

        [DisplayFormat(DataFormatString = "yyyy-MM-dd")]
        public DateTime? LastChecked { get; set; }
    }
}
