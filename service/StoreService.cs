using MySql.Data.MySqlClient;
using NLog;
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
    class StoreService
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public void DbExport(StoreData myData, MySqlDbConnection myDbCon)
        {
            MySqlDbConnection dbcon;

            string sqlcmd = "";

            // 引数にコネクションが指定されていた場合は指定されたコネクションを使用
            if (myDbCon != null)
                dbcon = myDbCon;
            else
                dbcon = new MySqlDbConnection();

            dbcon.openConnection();

            sqlcmd = "INSERT INTO av.store ( label, name1, name2, type, path, remark ) ";
            sqlcmd = sqlcmd + "VALUES( @Label, @Name1, @Name2, @Type, @Path, @Remark ) ";

            MySqlCommand command = new MySqlCommand(sqlcmd, dbcon.getMySqlConnection());

            List<MySqlParameter> listSqlParams = new List<MySqlParameter>();

            MySqlParameter sqlparam = new MySqlParameter("@Label", MySqlDbType.VarChar);
            sqlparam.Value = myData.Label;
            listSqlParams.Add(sqlparam);

            sqlparam = new MySqlParameter("@Name1", MySqlDbType.VarChar);
            sqlparam.Value = myData.Name1;
            listSqlParams.Add(sqlparam);

            sqlparam = new MySqlParameter("@Name2", MySqlDbType.VarChar);
            sqlparam.Value = myData.Name2;
            listSqlParams.Add(sqlparam);

            sqlparam = new MySqlParameter("@Type", MySqlDbType.VarChar);
            sqlparam.Value = myData.Type;
            listSqlParams.Add(sqlparam);

            sqlparam = new MySqlParameter("@Path", MySqlDbType.VarChar);
            sqlparam.Value = myData.Path;
            listSqlParams.Add(sqlparam);

            sqlparam = new MySqlParameter("@Remark", MySqlDbType.VarChar);
            sqlparam.Value = myData.Remark;
            listSqlParams.Add(sqlparam);

            command.Parameters.AddRange(listSqlParams.ToArray());

            command.ExecuteNonQuery();

            return;
        }

        public List<StoreData> GetList(MySqlDbConnection myDbCon)
        {
            List<StoreData> listData = new List<StoreData>();

            if (myDbCon == null)
                myDbCon = new MySqlDbConnection();

            string queryString
                        = "SELECT id"
                        + "    , label, name1, name2, type"
                        + "    , path, remark "
                        + "    , created_at, updated_at "
                        + "  FROM av.store "
                        + "";

            MySqlDataReader reader = null;
            try
            {
                reader = myDbCon.GetExecuteReader(queryString);

                do
                {

                    if (reader.IsClosed)
                    {
                        //_logger.Debug("reader.IsClosed");
                        throw new Exception("av.storeの取得でreaderがクローズされています");
                    }

                    while (reader.Read())
                    {
                        StoreData data = new StoreData();

                        data.Id = MySqlDbExportCommon.GetDbInt(reader, 0);
                        data.Label = MySqlDbExportCommon.GetDbString(reader, 1);
                        data.Name1 = MySqlDbExportCommon.GetDbString(reader, 2);
                        data.Name2 = MySqlDbExportCommon.GetDbString(reader, 3);
                        data.Type = MySqlDbExportCommon.GetDbString(reader, 4);
                        data.Path = MySqlDbExportCommon.GetDbString(reader, 5);
                        data.Remark = MySqlDbExportCommon.GetDbString(reader, 6);
                        data.CreatedAt = MySqlDbExportCommon.GetDbDateTime(reader, 7);
                        data.UpdatedAt = MySqlDbExportCommon.GetDbDateTime(reader, 8);

                        listData.Add(data);
                    }
                } while (reader.NextResult());
            }
            catch (Exception ex)
            {
                Debug.Write(ex);
            }
            finally
            {
                reader.Close();
            }

            reader.Close();

            myDbCon.closeConnection();

            return listData;
        }

    }
}
