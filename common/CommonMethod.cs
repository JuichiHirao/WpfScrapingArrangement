using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WpfScrapingArrangement.common
{
    class CommonMethod
    {
        public static string GetDisplaySize(long mySize)
        {
            double DoubleSize = 0;
            string Unit = "", SizeStr = "";
            double Long1024 = 1024;
            // M（メガ）
            if (mySize < Long1024 * Long1024)
            {
                DoubleSize = mySize / Long1024;
                Unit = "K";
            }
            else if (mySize < Long1024 * Long1024 * Long1024)
            {
                DoubleSize = mySize / Long1024 / Long1024;
                Unit = "M";
            }
            else if (mySize < Long1024 * Long1024 * Long1024 * Long1024)
            {
                DoubleSize = mySize / Long1024 / Long1024 / Long1024;
                Unit = "G";
            }
            else if (mySize < Long1024 * Long1024 * Long1024 * Long1024 * Long1024)
            {
                DoubleSize = mySize / Long1024 / Long1024 / Long1024 / Long1024;
                Unit = "T";
            }

            if (DoubleSize < 10)
                SizeStr = String.Format("{0:###,###,###,###.00#}{1}", DoubleSize, Unit);
            else if (DoubleSize < 100)
                SizeStr = String.Format("{0:###,###,###,###.0##}{1}", DoubleSize, Unit);
            else
                SizeStr = String.Format("{0:###,###,###,###}{1}", DoubleSize, Unit);

            return SizeStr;
        }

        public static string CheckRegex(string myPattern)
        {
            if (!String.IsNullOrEmpty(myPattern))
            {
                Regex regex = new Regex(myPattern);
                regex.Match("ABCD");
            }

            return myPattern;
        }
    }
}
