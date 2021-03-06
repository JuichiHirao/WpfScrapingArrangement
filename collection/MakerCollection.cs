﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WpfScrapingArrangement.collection
{
    class MakerCollection
    {
        public List<MovieMaker> listMaker;
        public ICollectionView ColViewListMakers;

        string searchConditionText;
        int[] searchConditionKinds;
        bool searchConditionAuto;

        public MakerCollection()
        {
            searchConditionText = null;
            searchConditionKinds = null;

            DataSet();
            ColViewListMakers = CollectionViewSource.GetDefaultView(listMaker);
        }

        public void DataSet()
        {
            listMaker = MovieMakers.GetAllData();
        }

        public void Refresh()
        {
            ColViewListMakers = null;
            listMaker = null;
            DataSet();
            ColViewListMakers = CollectionViewSource.GetDefaultView(listMaker);
        }

        private List<MovieMaker> MatchData(string myPasteText)
        {
            List<MovieMaker> matchMakerList = new List<MovieMaker>();
            foreach(MovieMaker data in listMaker)
            {
                if (data.MatchProductNumber == null || data.MatchProductNumber.Length <= 0)
                    continue;

                Regex regex = new Regex(data.MatchStr);
                if (data.MatchStr != null && data.MatchStr.Length > 0)
                {
                    if (regex.IsMatch(myPasteText))
                    {
                        if (!data.MatchProductNumber.ToLower().Equals("anything"))
                        {
                            Regex regexProductNumber = new Regex(data.MatchProductNumber);
                            if (regexProductNumber.IsMatch(myPasteText))
                                matchMakerList.Add(data);
                        }
                        else
                            matchMakerList.Add(data);
                    }
                }
            }

            return matchMakerList;
        }

        /// <summary>
        /// 既存のロジック（MovieMakersのメソッド）を少し修正
        /// </summary>
        /// <param name="myImportData"></param>
        /// <param name="myListMakers"></param>
        /// <returns></returns>
        public List<MovieMaker> GetMatchData(MovieImportData myImportData)
        {
            List<MovieMaker> matchMakerList = new List<MovieMaker>();

            // 品番っぽい文字列が存在する場合
            //if (myFileContents.Kind == MovieFileContents.KIND_AVRIP)
            if (myImportData.ProductNumber != null && myImportData.ProductNumber.Length > 0)
            {
                string[] label = myImportData.ProductNumber.Split('-');
                var matchdata = from makerdata in listMaker
                                where makerdata.MatchStr.ToUpper() == label[0].ToUpper() // + '-'
                                    && makerdata.MatchProductNumber.Length <= 0
                                select makerdata;

                if (matchdata.Count() > 0)
                {
                    foreach (MovieMaker m in matchdata)
                        matchMakerList.Add(m);
                }
            }

            if (matchMakerList.Count <= 0)
                matchMakerList = MatchData(myImportData.CopyText);

            return matchMakerList;
        }

        public void SetCondition(int[] mySearchKinds, string mySearchText)
        {
            searchConditionKinds = mySearchKinds;
            searchConditionText = mySearchText;
            searchConditionAuto = false;
        }

        public void SetAutoOnly()
        {
            searchConditionAuto = true;
            ColViewListMakers.SortDescriptions.Clear();
            ColViewListMakers.SortDescriptions.Add(new SortDescription("CreateDate", ListSortDirection.Descending));
        }

        public void Execute()
        {
            int maxCondition = 0;
            if (!string.IsNullOrEmpty(searchConditionText))
                maxCondition++;
            if (searchConditionKinds != null && searchConditionKinds.Length > 0)
                maxCondition++;

            int matchCnt = 0;
            ColViewListMakers.Filter = delegate (object o)
            {
                MovieMaker data = o as MovieMaker;

                if (data == null)
                    return false;

                if (searchConditionAuto)
                {
                    if (data.RegisterdBy != null && data.RegisterdBy.IndexOf("AUTO", StringComparison.OrdinalIgnoreCase) >= 0)
                        return true;
                    else
                        return false;
                }
                if (searchConditionText == null && searchConditionKinds == null)
                    return true;

                matchCnt = 0;
                if (!string.IsNullOrEmpty(searchConditionText))
                {
                    if (data.Name.ToUpper().IndexOf(searchConditionText) >= 0
                        || data.Label.ToUpper().IndexOf(searchConditionText) >= 0
                        || data .MatchStr.ToUpper().IndexOf(searchConditionText) >= 0)
                        matchCnt++;
                }

                if (searchConditionKinds != null && searchConditionKinds.Length > 0)
                {
                    foreach (int condKind in searchConditionKinds)
                    {
                        if (condKind == data.Kind)
                        {
                            matchCnt++;
                            break;
                        }
                    }
                }

                if (maxCondition <= matchCnt)
                    return true;

                return false;
            };

        }
    }
}
