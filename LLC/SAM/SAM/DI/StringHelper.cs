using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAM.DI
{
    public static class StringHelper
    {
        public static string ParseDate(string date)
        {
            DateTime ret = DateTime.MinValue;
            string str = null;
            if (DateTime.TryParse(date, out ret))
            {
                str = ret.ToString("f");
                return str;
            }

            return null;
        }

        public static int ParseInt(string i)
        {
            int ret = -1;
            int.TryParse(i, out ret);
            return ret;
        }

        public static double ParseDouble(string i)
        {
            double ret = -1.0;
            double.TryParse(i, out ret);
            return ret;
        }

        public static long ParseLong(string i)
        {
            long ret = -1;
            long.TryParse(i, out ret);
            return ret;
        }
    }
}
