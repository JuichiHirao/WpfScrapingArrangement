using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfScrapingArrangement.data;
using WpfScrapingArrangement.service;

namespace WpfScrapingArrangement.collection
{
    class StoreCollection
    {
        public List<StoreData> listData;
        //public ICollectionView ColViewListData;

        StoreService service;

        public StoreCollection()
        {
            service = new StoreService();
            DataSet();
        }

        public void DataSet()
        {
            listData = service.GetList(new MySqlDbConnection());
        }

        public void Add(StoreData myStoreData)
        {
            service.DbExport(myStoreData, new MySqlDbConnection());
            listData.Add(myStoreData);
        }

        public StoreData GetMatchByPath(string myPath)
        {
            var matchdata = from storedata in listData
                            where storedata.Path.ToUpper() == myPath.ToUpper()
                            select storedata;

            int cnt = matchdata.Count<StoreData>();
            if (cnt <= 0)
                throw new Exception("storeに[" + myPath + "]に対応するパスの登録がありません");

            if (cnt > 1)
                throw new Exception("storeに[" + myPath + "]に対応するパスが複数あります");

            return matchdata.First();
        }
        public StoreData GetMatchByLabel(string myLabel)
        {
            var matchdata = from storedata in listData
                            where storedata.Label.ToUpper() == myLabel.ToUpper()
                            select storedata;

            int cnt = matchdata.Count<StoreData>();
            if (cnt <= 0)
                throw new Exception("storeに[" + myLabel + "]に対応するLabelの登録がありません");

            if (cnt > 1)
                throw new Exception("storeに[" + myLabel + "]に対応するLabelが複数あります");

            return matchdata.First();
        }

    }
}
