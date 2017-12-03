using System;
using System.Collections.Generic;

namespace DbCore.Models
{
    public partial class Reports
    {
        public string Id { get; set; }
        public long? ContentSize { get; set; }
        public long? Mean { get; set; }
        public double? StandardDeviation { get; set; }
        public int? SdMaximum { get; set; }
        public string Link { get; set; }
        public string Stat { get; set; }
    }
}
