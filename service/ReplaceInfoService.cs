using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfScrapingArrangement.data;

namespace WpfScrapingArrangement.service
{
    class ReplaceInfoService
    {
        public static ReplaceInfoData.EnumType GetEnumType(MySqlDataReader myReader, int myColumnNo)
        {
            string myData = "";
            ReplaceInfoData.EnumType enmVal = ReplaceInfoData.EnumType.none;
            try
            {
                if (!myReader.IsDBNull(myColumnNo))
                    myData = myReader.GetString(myColumnNo);

                enmVal = (ReplaceInfoData.EnumType)Enum.Parse(typeof(ReplaceInfoData.EnumType), myData, true);
            }
            catch (Exception)
            {
                // 何もしない
            }
            return enmVal;
        }

        public static ReplaceInfoData.EnumSourceType GetEnumSourceType(MySqlDataReader myReader, int myColumnNo)
        {
            string myData = "";
            ReplaceInfoData.EnumSourceType enmVal = ReplaceInfoData.EnumSourceType.none;
            try
            {
                if (!myReader.IsDBNull(myColumnNo))
                    myData = myReader.GetString(myColumnNo);

                enmVal = (ReplaceInfoData.EnumSourceType)Enum.Parse(typeof(ReplaceInfoData.EnumSourceType), myData, true);
            }
            catch (Exception)
            {
                // 何もしない
            }
            return enmVal;
        }

        public List<ReplaceInfoData> GetTypeAll(ReplaceInfoData.EnumType myType, MySqlDbConnection myDbCon)
        {
            List<ReplaceInfoData> list = new List<ReplaceInfoData>();

            MySqlDbConnection dbcon;
            string sqlcmd = "";

            // 引数にコネクションが指定されていた場合は指定されたコネクションを使用
            if (myDbCon != null)
                dbcon = myDbCon;
            else
                dbcon = new MySqlDbConnection();

            sqlcmd = "SELECT id, type, source, destination, source_type, created_at, updated_at ";
            sqlcmd = sqlcmd + "FROM replace_info ";
            sqlcmd = sqlcmd + "WHERE type = @Type ";
            sqlcmd = sqlcmd + "ORDER BY created_at DESC";

            List<MySqlParameter> listSqlParams = new List<MySqlParameter>();

            MySqlCommand command = new MySqlCommand(sqlcmd, dbcon.getMySqlConnection());

            MySqlParameter sqlparam = new MySqlParameter("@Type", MySqlDbType.Enum);
            sqlparam.Value = myType;
            listSqlParams.Add(sqlparam);
            dbcon.SetParameter(listSqlParams.ToArray());

            MySqlDataReader reader = null;
            try
            {
                reader = myDbCon.GetExecuteReader(sqlcmd);

                if (reader.IsClosed)
                {
                    //_logger.Debug("reader.IsClosed");
                    throw new Exception("replace_infoの取得でreaderがクローズされています");
                }

                while (reader.Read())
                {
                    ReplaceInfoData data = new ReplaceInfoData();

                    data.Id = MysqlExportCommon.GetDbInt(reader, 0);
                    data.Type = GetEnumType(reader, 1);
                    data.Source = MysqlExportCommon.GetDbString(reader, 2);
                    data.Destination = MysqlExportCommon.GetDbString(reader, 3);
                    data.SourceType = GetEnumSourceType(reader, 4);
                    data.CreatedAt = MysqlExportCommon.GetDbDateTime(reader, 5);
                    data.UpdatedAt = MysqlExportCommon.GetDbDateTime(reader, 6);

                    list.Add(data);
                }
            }
            finally
            {
                reader.Close();
            }

            reader.Close();

            myDbCon.closeConnection();

            return list;
        }
    }
}
