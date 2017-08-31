using System;
using System.Reflection;

namespace SAM.Models.Reports
{
    public class InvalidLinksModel
    {
        public int Id { get; set; }

        public int Link { get; set; }

        public int AttemptCount { get; set; }

        public DateTime? DateLastFound { get; set; }

        public string Source { get; set; }

        public string Url { get; set; }

        public DateTime? DateLastChecked { get; set; }

        public object ElementAt(string column, int index)
        {
            if (column.Equals("url", StringComparison.CurrentCultureIgnoreCase))
            {
                return Url.Replace("http://", "").Replace("https://", "").Split('/')[0];
            }

            return GetType().GetProperties()[index].GetValue(this);
        }
    }
}
