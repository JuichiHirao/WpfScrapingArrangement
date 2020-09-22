using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfScrapingArrangement.data;

namespace WpfScrapingArrangement.service
{
    class MakerService
    {
        public MakerData GetById(long myId, MySqlDbConnection myDbCon)
        {
            MakerData data = new MakerData();

            MySqlDbConnection dbcon;
            string sqlcmd = "";

            // 引数にコネクションが指定されていた場合は指定されたコネクションを使用
            if (myDbCon != null)
                dbcon = myDbCon;
            else
                dbcon = new MySqlDbConnection();

            sqlcmd = "SELECT id, name, match_name, label, match_label, kind, match_str, match_product_number, site_kind, replace_words, p_number_gen, info_url, deleted, registered_by, created_at, updated_at ";
            sqlcmd = sqlcmd + "FROM maker WHERE id = " + myId + " ";
            sqlcmd = sqlcmd + "ORDER BY CREATED_AT DESC";

            MySqlDataReader reader = null;
            try
            {
                reader = myDbCon.GetExecuteReader(sqlcmd);

                if (reader.IsClosed)
                {
                    //_logger.Debug("reader.IsClosed");
                    throw new Exception("MOVIE_SITESTOREの取得でreaderがクローズされています");
                }

                if (reader.Read())
                {
                    data.Id = MysqlExportCommon.GetDbInt(reader, 0);
                    data.Name = MysqlExportCommon.GetDbString(reader, 1);
                    data.MatchName = MysqlExportCommon.GetDbString(reader, 2);
                    data.Label = MysqlExportCommon.GetDbString(reader, 3);
                    data.MatchLabel = MysqlExportCommon.GetDbString(reader, 4);
                    data.Kind  = MysqlExportCommon.GetDbInt(reader, 5);
                    data.MatchStr = MysqlExportCommon.GetDbString(reader, 6);
                    data.MatchProductNumber = MysqlExportCommon.GetDbString(reader, 7);
                    data.SiteKind = MysqlExportCommon.GetDbInt(reader, 8);
                    data.ReplaceWord = MysqlExportCommon.GetDbString(reader, 9);
                    data.ProductNumberGenerate = MysqlExportCommon.GetDbInt(reader, 10);
                    data.InformationUrl = MysqlExportCommon.GetDbString(reader, 11);
                    data.Deleted = MysqlExportCommon.GetDbInt(reader, 12);
                    data.RegisteredBy = MysqlExportCommon.GetDbString(reader, 13);
                    data.CreatedAt = MysqlExportCommon.GetDbDateTime(reader, 14);
                    data.UpdatedAt = MysqlExportCommon.GetDbDateTime(reader, 15);
                }
            }
            finally
            {
                reader.Close();
            }

            reader.Close();

            myDbCon.closeConnection();

            return data;
        }
        public void DbUpdate(MakerData myData, MySqlDbConnection myDbCon)
        {
            MySqlDbConnection dbcon;
            string sqlcmd = "";

            // 引数にコネクションが指定されていた場合は指定されたコネクションを使用
            if (myDbCon != null)
                dbcon = myDbCon;
            else
                dbcon = new MySqlDbConnection();

            sqlcmd = "UPDATE maker ";
            sqlcmd += "SET name = @Name ";
            sqlcmd += ", match_name = @MatchName ";
            sqlcmd += ", label = @Label ";
            sqlcmd += ", match_label = @MatchLabel ";
            sqlcmd += ", kind = @Kind ";
            sqlcmd += ", match_str = @MatchStr ";
            sqlcmd += ", match_product_number = @MatchProductNumber ";
            sqlcmd += ", site_kind = @SiteKind ";
            sqlcmd += ", replace_words = @ReplaceWord ";
            sqlcmd += ", p_number_gen = @ProductNumberGenerate ";
            sqlcmd += ", info_url = @InformationUrl ";
            sqlcmd += ", deleted = @Deleted ";
            sqlcmd += ", registered_by = @RegisteredBy ";
            sqlcmd += "WHERE id = @Id ";

            MySqlCommand command = new MySqlCommand(sqlcmd, dbcon.getMySqlConnection());

            List<MySqlParameter> listSqlParams = new List<MySqlParameter>();

            MySqlParameter sqlparam = new MySqlParameter("@Name", MySqlDbType.Text);
            sqlparam.Value = myData.Name;
            listSqlParams.Add(sqlparam);

            sqlparam = new MySqlParameter("@MatchName", MySqlDbType.VarChar);
            sqlparam.Value = myData.MatchName;
            listSqlParams.Add(sqlparam);

            sqlparam = new MySqlParameter("@Label", MySqlDbType.VarChar);
            sqlparam.Value = myData.Label;
            listSqlParams.Add(sqlparam);

            sqlparam = new MySqlParameter("@MatchLabel", MySqlDbType.VarChar);
            sqlparam.Value = myData.MatchLabel;
            listSqlParams.Add(sqlparam);

            sqlparam = new MySqlParameter("@Kind", MySqlDbType.Int32);
            sqlparam.Value = myData.Kind;
            listSqlParams.Add(sqlparam);

            sqlparam = new MySqlParameter("@MatchStr", MySqlDbType.VarChar);
            sqlparam.Value = myData.MatchStr;
            listSqlParams.Add(sqlparam);

            sqlparam = new MySqlParameter("@MatchProductNumber", MySqlDbType.VarChar);
            sqlparam.Value = myData.MatchProductNumber;
            listSqlParams.Add(sqlparam);

            sqlparam = new MySqlParameter("@SiteKind", MySqlDbType.Int32);
            sqlparam.Value = myData.SiteKind;
            listSqlParams.Add(sqlparam);

            sqlparam = new MySqlParameter("@ReplaceWord", MySqlDbType.Text);
            sqlparam.Value = myData.ReplaceWord;
            listSqlParams.Add(sqlparam);

            sqlparam = new MySqlParameter("@ProductNumberGenerate", MySqlDbType.Text);
            sqlparam.Value = myData.ProductNumberGenerate;
            listSqlParams.Add(sqlparam);

            sqlparam = new MySqlParameter("@InformationUrl", MySqlDbType.Text);
            sqlparam.Value = myData.InformationUrl;
            listSqlParams.Add(sqlparam);

            sqlparam = new MySqlParameter("@Deleted", MySqlDbType.Int16);
            sqlparam.Value = myData.Deleted;
            listSqlParams.Add(sqlparam);

            sqlparam = new MySqlParameter("@RegisteredBy", MySqlDbType.VarChar);
            sqlparam.Value = myData.RegisteredBy;
            listSqlParams.Add(sqlparam);

            sqlparam = new MySqlParameter("@Id", MySqlDbType.Int32);
            sqlparam.Value = myData.Id;
            listSqlParams.Add(sqlparam);

            dbcon.SetParameter(listSqlParams.ToArray());

            dbcon.execSqlCommand(sqlcmd);

            return;
        }

    }
}
