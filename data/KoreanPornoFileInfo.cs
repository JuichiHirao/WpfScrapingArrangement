using System;
using System.IO;
using WpfScrapingArrangement.common;

namespace WpfScrapingArrangement
{
    public class KoreanPornoFileInfo
    {
        public bool IsSelected { get; set; }
        public FileInfo FileInfo { get; set; }
        public string ChangeFilename { get; set; }

        public string DisplayChangeFilename { get; set; }
        public DateTime ChangeLastWriteTime { get; set; }

        public string DisplayFilename { get; set; }

        public string DisplaySize { get; set; }

        public KoreanPornoFileInfo(string myFileInfo)
        {
            FileInfo = new FileInfo(myFileInfo);

            DisplaySize = CommonMethod.GetDisplaySize(FileInfo.Length);
            DisplayFilename = FileInfo.Name;
        }
    }
}