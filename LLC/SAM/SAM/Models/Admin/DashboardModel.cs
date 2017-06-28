using System;

namespace SAM.Models.Admin
{
    public class DashboardModel
    {
        public long Objects { get; set; }

        public long Links { get; set; }

        public long InvalidLinks { get; set; }

        public long HtmlFiles { get; set; }

        public DateTime LastExtracted { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
