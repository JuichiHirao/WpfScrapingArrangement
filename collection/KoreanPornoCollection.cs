using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Data;
using WpfScrapingArrangement.service;

namespace WpfScrapingArrangement.collection
{
    class KoreanPornoCollection
    {
        public List<MovieImportData> listData;
        public ICollectionView ColViewListData;

        KoreanPornoService service;
        MovieImportService serviceImport;

        public string BasePath;

        public KoreanPornoCollection(string myPath, string myExportPath)
        {
            listData = new List<MovieImportData>();
            serviceImport = new service.MovieImportService();

            listData = serviceImport.GetList(new MySqlDbConnection());
            ColViewListData = CollectionViewSource.GetDefaultView(listData);

            BasePath = myPath;

            DataSet();
        }

        public void DataSet()
        {
            ColViewListData.Filter = delegate (object o)
            {
                MovieImportData data = o as MovieImportData;

                if (data.Kind == 5)
                    return true;

                return false;
            };

        }

        public void Delete(MovieImportData myData)
        {
            listData.Remove(myData);
            ColViewListData.Refresh();
        }
    }
}
