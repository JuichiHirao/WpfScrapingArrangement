﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WpfScrapingArrangement
{
    public class MovieImportData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public List<HdInfo> HdKindList = new List<HdInfo>();

        private void SetHdInfo()
        {
            HdKindList.Add(new HdInfo(1, "60fps HD", "\\[FHD60fps\\]"));
            HdKindList.Add(new HdInfo(2, "60fps HD", "FHD60fps"));
            HdKindList.Add(new HdInfo(3, "WMF FHD", "FHDwmf"));
            HdKindList.Add(new HdInfo(4, "WMF FHD", "\\[FHDwmf\\]"));
            HdKindList.Add(new HdInfo(5, "FHD", "\\[FHD\\]"));
            HdKindList.Add(new HdInfo(6, "FHD", "FHD"));
            HdKindList.Add(new HdInfo(7, "HD", "\\[HD\\]"));
        }
        public class HdInfo
        {
            public HdInfo(int myKind, string myName, string myStrRegex)
            {
                Kind = myKind;
                Name = myName;
                StrtRegex = myStrRegex;
            }
            public int Kind { get; set; }

            public string StrtRegex { get; set; }

            public string Name { get; set; }
        }

        public int Id { get; set; }

        public string StoreLabel { get; set; }

        public int FileId { get; set; }

        public string CopyText { get; set; }

        public int Kind { get; set; }

        public string MatchProduct { get; set; }

        public string ProductNumber { get; set; }

        public string GetFileSearchString()
        {
            if (ProductNumber.IndexOf("-", StringComparison.Ordinal) >= 0)
                return ProductNumber + " " + ProductNumber.Replace("-", "");
            else
                return ProductNumber;
        }

        private string _StrProductDate;
        public string StrProductDate
        {
            get
            {
                return _StrProductDate;
            }
            set
            {
                try
                {
                    DateTime dt = Convert.ToDateTime(value);
                    ProductDate = dt;
                }
                catch (Exception ex)
                {
                }

            }
        }
        public DateTime ProductDate { get; set; }

        public string StrMaker { get; set; }

        public MovieMaker Maker { get; set; }

        public void SetMaker(MovieMaker myMaker)
        {
            if (myMaker != null)
                Maker = myMaker;

            if (Maker != null)
            {
                Kind = Maker.Kind;
                StrMaker = Maker.GetNameLabel();
            }

            if (ProductNumber == null || ProductNumber.Length <= 0)
            {
                Regex regex = new Regex(Maker.MatchProductNumber);
                ProductNumber = regex.Match(CopyText).Groups[0].Value;
            }
        }

        /// <summary>
        /// KINDが1以外でProductNumberが取得できなかった場合に、メーカーが取得出来た場合に本メソッドを実行してCopyTextからProductNumberを取り出す
        /// </summary>
        public void SetProductNumber()
        {
            if (Maker != null)
            {
                Regex regex = new Regex(Maker.MatchProductNumber);

                foreach (Match m in regex.Matches(CopyText))
                {
                    ProductNumber = m.Value;
                    break;
                }
            }
        }

        public string GetMaker()
        {
            if (Maker != null)
                return Maker.GetNameLabel();

            return StrMaker;
        }

        public string GetMatchMaker()
        {
            if (Maker != null)
                return Maker.MatchStr;

            if (ProductNumber != null)
                return Regex.Replace(ProductNumber, "-.*", "");

            return "";
        }

        public string Title { get; set; }

        public string Actresses { get; set; }

        public HdInfo HdKind { get; set; }

        private bool _HdFlag;
        public bool? HdFlag
        {
            get
            {
                return _HdFlag;
            }
            set
            {
                if (value == null)
                    _HdFlag = false;
                else
                {
                    _HdFlag = (bool)value;
                }
            }
        }

        public void SetHdKind(int myKind)
        {
            if (myKind == 0)
            {
                _HdFlag = false;
                HdKind = null;
            }

            foreach (HdInfo hdInfo in HdKindList)
            {
                if (hdInfo.Kind == myKind)
                {
                    HdKind = hdInfo;
                    _HdFlag = true;
                    break;
                }
            }

            return;
        }

        public string DisplaySplitFlag { get; set; }

        private bool _SplitFlag;
        public bool? SplitFlag
        {
            get
            {
                return _SplitFlag;
            }
            set
            {
                if (value == null)
                {
                    _SplitFlag = false;
                    DisplaySplitFlag = "";
                }
                else
                {
                    _SplitFlag = (bool)value;

                    if (_SplitFlag)
                        DisplaySplitFlag = "○";
                }
            }
        }

        public string DisplayRarFlag { get; set; }

        private bool _RarFlag;
        public bool? RarFlag
        {
            get
            {
                return _RarFlag;
            }
            set
            {
                if (value == null)
                {
                    _RarFlag = false;
                    DisplayRarFlag = "";
                }
                else
                {
                    _RarFlag = (bool)value;
                    if (_RarFlag)
                        DisplayRarFlag = "○";
                }
            }
        }

        public string DisplayNameOnlyFlag { get; set; }

        private bool _NameOnlyFlag;
        public bool? NameOnlyFlag
        {
            get
            {
                return _NameOnlyFlag;
            }
            set
            {
                if (value == null)
                {
                    _NameOnlyFlag = false;
                    DisplayNameOnlyFlag = "";
                }
                else
                {
                    _NameOnlyFlag = (bool)value;
                    if (_NameOnlyFlag)
                        DisplayNameOnlyFlag = "○";
                }
            }
        }

        public string Tag { get; set; }

        public int Rating { get; set; }

        public string DisplayHdKind { get; set;}

        private string _Filename;
        public string Filename
        {
            get
            {
                return _Filename;
            }
            set
            {
                _Filename = value;
                foreach (HdInfo data in HdKindList)
                {
                    Regex regex = new Regex(data.Name);
                    if (regex.IsMatch(Filename.Trim()))
                    {
                        DisplayHdKind = data.Name;
                        break;
                    }
                }

            }
        }

        public long Size { get; set; }

        public string Package { get; set; }

        public string Thumbnail { get; set; }

        public string DownloadFiles { get; set; }

        public DateTime JavPostDate { get; set; }

        public string SearchResult { get; set; }

        public string Detail { get; set; }

        public long JavId { get; set; }

        public string JavUrl { get; set; }

        protected bool _IsTarget;
        public bool IsTarget
        {
            get
            {
                return _IsTarget;
            }
            set
            {
                _IsTarget = value;
                NotifyPropertyChanged("IsTarget");
            }
        }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public void GenerateFilename()
        {
            string strDt = ProductDate.ToString("yyyMMdd");

            string name = "";
            if (Kind == 1)
                name += "[AVRIP]";
            else if (Kind == 2)
                name += "[IVRIP]";
            else if (Kind == 3)
                name += "[裏AVRIP]";
            else if (Kind == 4)
                name += "[DMMR-AVRIP]";
            else if (Kind == 5)
                name += "[DMMR-IVRIP]";

            string maker = "";
            if (Maker != null)
            {
                maker = Maker.GetNameLabel();
            }
            else
                maker = StrMaker;

            name += "【" + maker + "】";
            name += Title + " ";
            name += "[" + ProductNumber + " " + strDt + "]";

            Filename = name;
        }

        public MovieImportData()
        {
            HdKind = null;
            SetHdInfo();
        }

        public string GetFilterProductNumber()
        {
            string searchText = "";

            if (ProductNumber != null && ProductNumber.Length > 0)
            {
                string HyphenStr = ProductNumber;
                string HyphenWithoutStr = HyphenStr.Replace("-", "");

                if (HyphenStr.Equals(HyphenWithoutStr))
                    searchText = HyphenStr;
                else
                    searchText = HyphenStr + " " + HyphenWithoutStr;
            }

            return searchText;
        }

        public MovieImportData(string myPasteText)
        {
            SetHdInfo();
            string pasteText = "";

            if (myPasteText.IndexOf("RAR") == 0)
            {
                pasteText = myPasteText.Replace("RAR ", "");
                _RarFlag = true;
            }
            else
                pasteText = myPasteText;

            if (pasteText.IndexOf("[AVRIP]") == 0
                || pasteText.IndexOf("[IVRIP]") == 0
                || pasteText.IndexOf("[裏AVRIP]") == 0
                || pasteText.IndexOf("[DMMR-AVRIP]") == 0
                || pasteText.IndexOf("[DMMR-IVRIP]") == 0)
            {
                Regex regexDate = new Regex("[12][0-9][0-9][0-9][01][0-9][0-3][0-9]");
                if (regexDate.IsMatch(pasteText))
                {
                    MatchCollection mc = regexDate.Matches(pasteText);
                    string strDate = mc[0].Value.ToString();

                    string[] expectedFormat = { "yyyyMMdd" };
                    ProductDate = System.DateTime.ParseExact(strDate,
                                expectedFormat,
                                System.Globalization.DateTimeFormatInfo.InvariantInfo,
                                System.Globalization.DateTimeStyles.None);

                    StrProductDate = ProductDate.ToString("yyyy/MM/dd");
                }

                Regex regexProductNumber = new Regex("^.* ");
                int lastPos = pasteText.LastIndexOf("[");
                string str = pasteText.Substring(lastPos+1);
                if (regexProductNumber.IsMatch(str))
                {
                    MatchCollection mc = regexProductNumber.Matches(str);
                    ProductNumber = mc[0].Value.ToString().Trim();
                }

                if (pasteText.IndexOf("[AVRIP]") == 0)
                    Kind = 1;
                else if (pasteText.IndexOf("[IVRIP]") == 0)
                    Kind = 2;
                else if (pasteText.IndexOf("[裏AVRIP]") == 0)
                    Kind = 3;

                int posFrom = pasteText.IndexOf("【");
                int posTo = pasteText.IndexOf("】");
                if (posFrom >= 0)
                {
                    StrMaker = pasteText.Substring(posFrom + 1, (posTo - posFrom) - 1);
                    Maker = null;
                }

                Title = pasteText.Substring(posTo+1, (lastPos - posTo)-1).Trim();

                int acPos = pasteText.Substring(lastPos).IndexOf("（");
                if (acPos >= 0)
                    Actresses = pasteText.Substring(lastPos).Replace("（", "").Replace("）", "");
            }

            return;
        }

        private string RemoveTextBefore(string myText)
        {
            return Regex.Replace(CopyText, ".*" + Regex.Escape(myText), "", RegexOptions.IgnoreCase).Trim();
        }

        private string RemoveTextAfterDate(string myText, bool myIsAfterCut)
        {
            string dateSprintString = Regex.Escape("/-");
            if (myIsAfterCut)
                return Regex.Replace(myText, " " + "[12][[0123][0-9][0-9][" + dateSprintString + "][0-1][0-9][" + dateSprintString + "][0-3][0-9].*", "").Trim();

            return Regex.Replace(myText, "[12][[0123][0-9][0-9][" + dateSprintString + "][0-1][0-9][" + dateSprintString + "][0-3][0-9]", "").Trim();
            //return Regex.Replace(myText, ". " + "[12][[0123][0-9][0-9][" + dateSprintString + "][0-1][0-9][" + dateSprintString + "][0-3][0-9]. ", "").Trim();
        }

        public void SetPickupTitle(MovieFileContents myFileContents)
        {
            MovieImportData impData = new MovieImportData(myFileContents.Name);

            string workCopyText = "";
            // クリップボードテキストから不要な削除文字列を設定する
            //List<string> listCutText = new List<string>();

            ProductNumber = impData.ProductNumber;

            if (Maker != null)
            {
                Regex regex = new Regex(Maker.MatchProductNumber);
                if (regex.IsMatch(workCopyText.Trim()))
                {
                    foreach (Match m in regex.Matches(workCopyText))
                        workCopyText = impData.Title.Replace(m.Value.ToString(), "");
                }
            }
            else
            {
                workCopyText = impData.Title;
            }

            foreach (HdInfo data in HdKindList)
            {
                Regex regex = new Regex(data.Name + "$");
                if (regex.IsMatch(workCopyText.Trim()))
                {
                    foreach (Match m in regex.Matches(workCopyText))
                        workCopyText = workCopyText.Replace(m.Value.ToString(), "");
                }
            }

            string hd = "";
            if (HdKind != null)
                hd = " " + HdKind.Name;

            if (workCopyText.Length > 0)
                Title = workCopyText + hd;
            else
                Title = impData.Title + hd;
        }

        public void SetPickupTitle()
        {
            string workCopyText = CopyText;
            // クリップボードテキストから不要な削除文字列を設定する
            List<string> listCutText = new List<string>();

            if (ProductNumber != null && ProductNumber.Length > 0)
                workCopyText = RemoveTextBefore(ProductNumber);

            if (ProductDate.Year <= 1900)
                ParseSetSellDate(CopyText);

            if (ProductDate.Year > 1900)
                workCopyText = RemoveTextAfterDate(workCopyText, true);

            if (Maker != null && Maker.MatchProductNumber.Equals("anything"))
            {
                workCopyText = Regex.Replace(workCopyText, Maker.MatchStr, "").Trim();
                workCopyText = RemoveTextAfterDate(workCopyText, false);
            }
            else if (Maker != null)
                workCopyText = Regex.Replace(workCopyText, Maker.MatchStr, "").Trim();

            foreach (HdInfo data in HdKindList)
            {
                Regex regex = new Regex(data.Name + "$");
                if (regex.IsMatch(workCopyText.Trim()))
                {
                    foreach (Match m in regex.Matches(workCopyText))
                        workCopyText = workCopyText.Replace(m.Value.ToString(), "");
                }
            }

            string hd = "";
            if (HdKind != null)
                hd = " " + HdKind.Name;

            Title = workCopyText + hd;
        }

        public void ParseFromPasteText(string myPasteText)
        {
            CopyText = myPasteText;

            // 日付を取得
            string matchProductDate = ParseSetSellDate(CopyText);

            string editText = "";
            if (matchProductDate != null && matchProductDate.Length > 0)
                editText = CopyText.Trim().Replace(matchProductDate, "");
            else
                editText = CopyText.Trim();

            Regex regexHd = null;
            foreach (HdInfo hd in HdKindList)
            {
                regexHd = new Regex(hd.StrtRegex);

                if (regexHd.IsMatch(myPasteText))
                {
                    editText = editText.Replace(regexHd.Match(myPasteText).Value.ToString(), "");
                    editText.Trim();
                    HdKind = hd;
                    break;
                }
            }

            // 日付に「-」ハイフンがある場合は品番の区切りと間違えるので日付部分を削除
            //if (MatchStrSellDate.IndexOf("-") >= 0)
            //    EditPasteText = myText.Replace(MatchStrSellDate, "");

            // テキスト内の日付の後ろ文字列は女優名として取得
            ParseSetActress(editText, matchProductDate);

            // 品番を設定
            ParseSetProductNumber(editText);
        }

        public string ParseSetSellDate(string myText)
        {
            if (myText == null || myText.Length <= 0)
                return "";

            Regex regexDate = new Regex("[12][0-9][0-9][0-9][/-][0-1]{0,1}[0-9][/-][0-9]{0,1}[0-9]");

            string matchStr = "";
            if (regexDate.IsMatch(myText))
            {
                matchStr = regexDate.Match(myText).Value.ToString();
                try
                {
                    ProductDate = Convert.ToDateTime(matchStr);
                    //DispSellDate = ProductDate.ToString("yyyyMMdd");
                }
                catch (Exception)
                {
                    // 何もしない
                }
            }

            return matchStr;
        }
        public void ParseSetActress(string myText, string myMatchProductDate)
        {
            string matchStr = "";
            if (myMatchProductDate != null && myMatchProductDate.Length > 0)
            {
                matchStr = Regex.Match(myText, myMatchProductDate + "(.*)").Groups[0].Value;
                matchStr = matchStr.Replace(myMatchProductDate, "").Trim();
            }

            Actresses = ConvertActress(matchStr, "、");
            Tag = ConvertActress(matchStr, ",");
        }

        public void ParseSetProductNumber(string myText)
        {
            Regex regex = new Regex("\\[{0,1}[0-9A-Za-z]{1,}-[0-9]*\\]{0,1}[A-Za-z]{0,1}");

            string matchStr = "";
            // 品番っぽい文字列が存在する場合は暫定でAVRIPを設定
            if (regex.IsMatch(myText))
            {
                matchStr = regex.Match(myText).Value.ToString();
                if (matchStr.Equals("S-C"))
                    matchStr = "";
                ProductNumber = matchStr.Replace("[", "").Replace("]", "").ToUpper();
            }
            // 品番っぽいのが無い場合は、数字のみで品番を取得
            else
            {
                Regex regexP1 = new Regex("[0-9]*[_-][0-9]*");
                Regex regexP2 = new Regex(" [0-9]*");
                Regex regexP3 = new Regex(" [A-Za-z]*[0-9]*");

                if (regexP1.IsMatch(myText))
                    matchStr = regexP1.Match(myText).Value.ToString().Trim();
                else if (regexP2.IsMatch(myText))
                    matchStr = regexP2.Match(myText).Value.ToString().Trim();
                else if (regexP3.IsMatch(myText))
                    matchStr = regexP3.Match(myText).Value.ToString().Trim().ToUpper();

                Kind = 0;
                if (matchStr != null && matchStr.Length > 0)
                    ProductNumber = matchStr;
            }
        }
        public string ConvertActress(string myText, string mySeparator)
        {
            string[] arrSplit = { "／", " ", ",", "　", "・" };
            string sepa = "";
            string[] arrActress = null;
            string actresses = "";

            string separetor = "";
            if (mySeparator == null || mySeparator.Length <= 0)
                separetor = "、";
            else
                separetor = mySeparator;

            foreach (string split in arrSplit)
            {
                string[] arrsepa = { split };
                arrActress = myText.Split(arrsepa, StringSplitOptions.None);
                if (arrActress.Length > 1)
                {
                    //MatchStrActresses = myText;
                    sepa = split;
                    break;
                }
            }
            if (sepa.Length <= 0)
                actresses = myText.Trim();
            else
            {
                foreach (string actress in arrActress)
                {
                    string tmpActres = actress.Replace("?", "").Trim();

                    if (tmpActres.Length > 0)
                    {
                        if (actresses.Length > 0)
                            actresses += separetor;
                        actresses += actress;
                    }
                }
            }

            return actresses;
        }
    }
}
