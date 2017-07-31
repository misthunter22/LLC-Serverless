using System.Collections.Generic;

namespace SAM.Models.Admin
{
    public class DashboardModel
    {
        public string Source { get; set; }

        public string Title { get; set; }

        public List<DashboardData> Data { get; set; }
    }

    public class DashboardData
    {
        public string Key { get; set; }

        public string Data { get; set; }
    }
}
