using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;

namespace WpfScrapingArrangement
{
    public class MovieMaker
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public int Kind { get; set; }
        public string MatchStr { get; set; }
        public string MatchProductNumber { get; set; }
        public string MatchProductNumberValue { get; set; }
        public string RegisterdBy { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        public string GetNameLabel()
        {
            string namelabel = "";
            if (Label == null || Label.Length <= 0)
                namelabel = Name;
            else
                namelabel = Name + "：" + Label;

            return namelabel;
        }

        public void ParseFromFileName(MovieFileContents myFile)
        {
            if (myFile.Name.IndexOf("[AVRIP]") == 0)
                Kind = MovieFileContents.KIND_AVRIP;
            else if (myFile.Name.IndexOf("[IVRIP]") == 0)
                Kind = MovieFileContents.KIND_IVRIP;
            else if (myFile.Name.IndexOf("[裏AVRIP]") == 0)
                Kind = MovieFileContents.KIND_URAAVRIP;

            Regex regex = new Regex("【(.*)】");

            if (regex.IsMatch(myFile.Name))
            {
                string work = regex.Match(myFile.Name).Groups[1].Value.ToString();

                string[] arrSplitStr = {"："};
                string[] arrMakerNames = work.Split(arrSplitStr, StringSplitOptions.None);
                if (arrMakerNames.Length >= 2)
                {
                    Name = arrMakerNames[0];
                    Label = arrMakerNames[1];
                }
                else
                    Name = work;
            }
        }

        public void DbUpdate(MySqlDbConnection myDbCon)
        {
            MySqlDbConnection dbcon;
            string sqlcmd = "";

            // 引数にコネクションが指定されていた場合は指定されたコネクションを使用
            if (myDbCon != null)
                dbcon = myDbCon;
            else
                dbcon = new MySqlDbConnection();

            sqlcmd = "UPDATE maker ";
            sqlcmd = sqlcmd + "SET NAME = @NAME, LABEL = @LABEL, KIND = @KIND, MATCH_STR = @MATCH_STR, MATCH_PRODUCT_NUMBER = @MATCH_PRODUCT_NUMBER ";
            sqlcmd = sqlcmd + "WHERE ID = @ID ";

            MySqlCommand scmd = new MySqlCommand(sqlcmd, dbcon.getMySqlConnection());
            DataTable dtSaraly = new DataTable();

            List<MySqlParameter> sqlparams = new List<MySqlParameter>();

            MySqlParameter sqlparam = new MySqlParameter("@NAME", MySqlDbType.VarChar);
            sqlparam.Value = Name;
            sqlparams.Add(sqlparam);

            sqlparam = new MySqlParameter("@LABEL", MySqlDbType.VarChar);
            sqlparam.Value = Label;
            sqlparams.Add(sqlparam);

            sqlparam = new MySqlParameter("@KIND", MySqlDbType.Int32);
            sqlparam.Value = Kind;
            sqlparams.Add(sqlparam);

            sqlparam = new MySqlParameter("@MATCH_STR", MySqlDbType.VarChar);
            sqlparam.Value = MatchStr;
            sqlparams.Add(sqlparam);

            sqlparam = new MySqlParameter("@MATCH_PRODUCT_NUMBER", MySqlDbType.VarChar);
            sqlparam.Value = MatchProductNumber;
            sqlparams.Add(sqlparam);

            sqlparam = new MySqlParameter("@ID", MySqlDbType.Int32);
            sqlparam.Value = Id;
            sqlparams.Add(sqlparam);

            dbcon.SetParameter(sqlparams.ToArray());

            dbcon.execSqlCommand(sqlcmd);

            return;
        }

        public void DbExport(MySqlDbConnection myDbCon)
        {
            MySqlDbConnection dbcon;
            string sqlcmd = "";

            // 引数にコネクションが指定されていた場合は指定されたコネクションを使用
            if (myDbCon != null)
                dbcon = myDbCon;
            else
                dbcon = new MySqlDbConnection();

            sqlcmd = "INSERT INTO maker ( NAME, LABEL, KIND, MATCH_STR, MATCH_PRODUCT_NUMBER ) ";
            sqlcmd = sqlcmd + "VALUES( @NAME, @LABEL, @KIND, @MATCH_STR, @MATCH_PRODUCT_NUMBER ) ";

            MySqlCommand scmd = new MySqlCommand(sqlcmd, dbcon.getMySqlConnection());
            DataTable dtSaraly = new DataTable();

            MySqlParameter[] sqlparams = new MySqlParameter[5];

            sqlparams[0] = new MySqlParameter("@NAME", SqlDbType.VarChar);
            sqlparams[0].Value = Name;
            sqlparams[1] = new MySqlParameter("@LABEL", SqlDbType.VarChar);
            sqlparams[1].Value = Label;
            sqlparams[2] = new MySqlParameter("@KIND", SqlDbType.Int);
            sqlparams[2].Value = Kind;
            sqlparams[3] = new MySqlParameter("@MATCH_STR", SqlDbType.VarChar);
            sqlparams[3].Value = MatchStr;
            sqlparams[4] = new MySqlParameter("@MATCH_PRODUCT_NUMBER", SqlDbType.VarChar);
            sqlparams[4].Value = MatchProductNumber;

            dbcon.SetParameter(sqlparams);

            dbcon.execSqlCommand(sqlcmd);

            return;
        }

        public void DbDelete(DbConnection myDbCon)
        {
            string DeleteCommand = "";

            DeleteCommand = "DELETE MOVIE_MAKERS WHERE ID = @pId ";

            //Debug.Print(InsertCommand);

            SqlParameter[] sqlparams = new SqlParameter[1];

            // ID
            sqlparams[0] = new SqlParameter("@pId", SqlDbType.Int);
            sqlparams[0].Value = Id;

            myDbCon.SetParameter(sqlparams);

            // DELETE文の実行
            int cnt = myDbCon.execSqlCommand(DeleteCommand);

            if (cnt == 0)
                throw new Exception("ID[" + Id + "]の削除対象のデータが存在しません");
        }
    }
}
