using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfScrapingArrangement.data
{
    class StoreData
    {
        public StoreData()
        {
            Label = "";
            Name1 = "";
            Name2 = "";
            Path = "";
            Remark = "";
        }

        public int Id { get; set; }

        private string _Label;
        public string Label
        {
            get
            {
                return _Label;
            }
            set
            {
                _Label = value;
            }
        }
        public string Name1 { get; set; }
        public string Name2 { get; set; }
        public string Type { get; set; }

        private string _Path;
        public string Path
        {
            get
            {
                return _Path;
            }
            set
            {
                _Path = value;
            }
        }
        public string Remark { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
}
