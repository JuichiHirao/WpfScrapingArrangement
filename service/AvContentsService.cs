using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfScrapingArrangement.common;
using WpfScrapingArrangement.data;

namespace WpfScrapingArrangement.service
{
    class AvContentsService
    {
        public long GetStoreLabelTotalSize(string myStoreLabel, MySqlDbConnection myDbCon)
        {
            if (myDbCon == null)
                myDbCon = new MySqlDbConnection();

            string queryString = "SELECT sum(size) FROM av.v_contents WHERE store_label = @StoreLabel ";

            long total = 0;
            try
            {
                List<MySqlParameter> listSqlParam = new List<MySqlParameter>();

                MySqlParameter sqlparam = new MySqlParameter("@StoreLabel", MySqlDbType.VarChar);
                sqlparam.Value = myStoreLabel;
                listSqlParam.Add(sqlparam);

                myDbCon.SetParameter(listSqlParam.ToArray());
                total = myDbCon.getLongSql(queryString);
            }
            catch (Exception ex)
            {
                Debug.Write(ex);
            }
            //Debug.Print("totalsize " + total);

            myDbCon.closeConnection();

            return total;
        }

        public string GetActressRating(string myTag, MySqlDbConnection myDbCon)
        {
            if (myDbCon == null)
                myDbCon = new MySqlDbConnection();

            List<AvContentsData> listMContents = new List<AvContentsData>();
            string queryString = "SELECT tag, rating FROM av.v_contents WHERE tag like @Tag ";

            MySqlDataReader reader = null;
            int total = 0;
            int activeTotal = 0;
            int rating = 0;
            int maxRating = 0;
            try
            {
                List<MySqlParameter> listSqlParam = new List<MySqlParameter>();

                MySqlParameter sqlparam = new MySqlParameter("@Tag", MySqlDbType.VarChar);
                sqlparam.Value = String.Format("{0}", myTag);
                listSqlParam.Add(sqlparam);
                myDbCon.SetParameter(listSqlParam.ToArray());

                reader = myDbCon.GetExecuteReader(queryString);

                do
                {
                    myDbCon.SetParameter(listSqlParam.ToArray());

                    if (reader.IsClosed)
                    {
                        //_logger.Debug("av.contents reader.IsClosed");
                        throw new Exception("av.contentsの取得でreaderがクローズされています");
                    }

                    while (reader.Read())
                    {
                        AvContentsData data = new AvContentsData();

                        data.Tag = MySqlDbExportCommon.GetDbString(reader, 0);
                        data.Rating = MySqlDbExportCommon.GetDbInt(reader, 1);

                        if (data.Rating > 0)
                            activeTotal++;

                        if (maxRating < data.Rating)
                            maxRating = data.Rating;

                        total++;
                    }
                } while (reader.NextResult());
            }
            catch (Exception ex)
            {
                Debug.Write(ex);
            }
            finally
            {
                if (reader != null) reader.Close();
            }

            myDbCon.closeConnection();

            return String.Format("max{0} 済{1}/全{2}", maxRating, activeTotal, total);
        }

    }
}
