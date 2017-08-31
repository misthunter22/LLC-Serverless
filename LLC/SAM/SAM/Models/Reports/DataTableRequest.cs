/**
 * No Includes 
 */

namespace SAM.Models.Reports
{
    public class DataTableRequest
    {
        public DataTableRequest()
        {
            draw   = -1;
            length = 10;
            start  = 0;
            column = 0;
            direction = "asc";
        }

        public int draw { get; set; }

        public int length { get; set; }

        public int start { get; set; }

        public int column { get; set; }

        public string columnName { get; set; }

        public string direction { get; set; }

        public string search { get; set; }
    }
}
