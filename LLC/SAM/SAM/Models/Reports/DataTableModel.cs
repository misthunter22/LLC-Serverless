using System.Collections.Generic;

namespace SAM.Models.Reports
{
    public class DataTableModel<T>
    {
        public int draw { get; set; }

        public int length { get; set; }

        public int start { get; set; }

        public int recordsTotal { get; set; }

        public int recordsFiltered { get; set; }

        public List<T> data { get; set; }
    }
}
