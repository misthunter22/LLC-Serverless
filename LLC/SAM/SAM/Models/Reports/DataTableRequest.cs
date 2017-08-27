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
        }

        public int draw { get; set; }

        public int length { get; set; }

        public int start { get; set; }
    }
}
