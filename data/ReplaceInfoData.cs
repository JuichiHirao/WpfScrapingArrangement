using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfScrapingArrangement.data
{
    class ReplaceInfoData
    {
        public enum EnumType
        {
            none,
            actress,
            maker_name,
            maker_m_name,
            maker_label,
            maker_m_label,
            maker_site
        }

        public enum EnumSourceType
        {
            none,
            text,
            re
        }

        public int Id { get; set; }

        public Enum Type { get; set; }

        public string Source { get; set; }

        public string Destination { get; set; }

        public Enum SourceType { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
