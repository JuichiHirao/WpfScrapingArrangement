using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfScrapingArrangement.data
{
    class MakerData
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string MatchName { get; set; }
        public string Label { get; set; }

        public string MatchLabel { get; set; }

        public int Kind { get; set; }

        public string MatchStr { get; set; }

        public string MatchProductNumber { get; set; }

        public int SiteKind { get; set; }

        public string ReplaceWord { get; set; }

        public int ProductNumberGenerate { get; set;}

        public string InformationUrl { get; set; }

        public int Deleted { get; set; }

        public string RegisteredBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

    }
}
