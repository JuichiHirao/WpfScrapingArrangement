using System.Text;
using System.IO;
using System.Data.SqlTypes;

namespace WpfScrapingArrangement
{
    public class ActionInfo
    {
        public static int EXEC_KIND_COPY_LASTWRITE = 1;
        public static int EXEC_KIND_MOVE = 2;

        public int Kind { get; set; }
        public FileInfo fileSource { get; set; }
        public FileInfo fileDestination { get; set; }
    }
}
