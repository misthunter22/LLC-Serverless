using System;
using System.Reflection;

namespace SAM.Models.Reports
{
    public class WarningLinksModel
    {
        public int Id { get; set; }

        public int ContentSize { get; set; }

        public int Mean { get; set; }

        public long StandardDeviation { get; set; }

        public int SdRange{ get; set; }

        public int LinkId { get; set; }

        public int StatId { get; set; }

        public string Url { get; set; }
    }
}
