using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace WpfScrapingArrangement.service
{
    class MovieImportService
    {
        public MovieImportData DbExport(MovieImportData myData, MySqlDbConnection myDbCon)
        {
            MySqlDbConnection dbcon;

            string sqlcmd = "";

            // 引数にコネクションが指定されていた場合は指定されたコネクションを使用
            if (myDbCon != null)
                dbcon = myDbCon;
            else
                dbcon = new MySqlDbConnection();

            dbcon.openConnection();

            sqlcmd = "INSERT INTO import ( COPY_TEXT, KIND, MATCH_PRODUCT, PRODUCT_NUMBER, PRODUCT_DATE, MAKER, TITLE, ACTRESSES, RAR_FLAG, SPLIT_FLAG, NAME_ONLY_FLAG, TAG, FILENAME, HD_KIND, MOVIE_FILES_ID, PACKAGE, THUMBNAIL, DOWNLOAD_FILES ) ";
            sqlcmd = sqlcmd + "VALUES( @CopyText, @Kind, @MatchProduct, @ProductNumber, @ProductDate, @Maker, @Title, @Actresses, @RarFlag, @SplitFlag, @NameOnlyFlag, @Tag, @Filename, @HdKind, @MovieFilesId, @Package, @Thumbnail ) ";

            MySqlCommand command = new MySqlCommand(sqlcmd, dbcon.getMySqlConnection());
            DataTable dtSaraly = new DataTable();

            List<MySqlParameter> listSqlParams = new List<MySqlParameter>();

            MySqlParameter sqlparam = new MySqlParameter("@CopyText", MySqlDbType.VarChar);
            sqlparam.Value = myData.CopyText;
            listSqlParams.Add(sqlparam);

            sqlparam = new MySqlParameter("@Kind", MySqlDbType.Int32);
            sqlparam.Value = myData.Kind;
            listSqlParams.Add(sqlparam);

            sqlparam = new MySqlParameter("@MatchProduct", MySqlDbType.VarChar);
            sqlparam.Value = myData.MatchProduct;
            listSqlParams.Add(sqlparam);

            sqlparam = new MySqlParameter("@ProductNumber", MySqlDbType.VarChar);
            sqlparam.Value = myData.ProductNumber;
            listSqlParams.Add(sqlparam);

            sqlparam = new MySqlParameter("@ProductDate", MySqlDbType.DateTime);
            sqlparam.Value = myData.ProductDate;
            listSqlParams.Add(sqlparam);

            sqlparam = new MySqlParameter("@Maker", MySqlDbType.VarChar);
            sqlparam.Value = myData.GetMaker();
            listSqlParams.Add(sqlparam);

            sqlparam = new MySqlParameter("@Title", MySqlDbType.VarChar);
            sqlparam.Value = myData.Title;
            listSqlParams.Add(sqlparam);

            sqlparam = new MySqlParameter("@Actresses", MySqlDbType.VarChar);
            sqlparam.Value = myData.Actresses;
            listSqlParams.Add(sqlparam);

            sqlparam = new MySqlParameter("@RarFlag", MySqlDbType.Int16);
            sqlparam.Value = myData.RarFlag;
            listSqlParams.Add(sqlparam);

            sqlparam = new MySqlParameter("@SplitFlag", MySqlDbType.Int16);
            sqlparam.Value = myData.SplitFlag;
            listSqlParams.Add(sqlparam);

            sqlparam = new MySqlParameter("@NameOnlyFlag", MySqlDbType.Int16);
            sqlparam.Value = myData.NameOnlyFlag;
            listSqlParams.Add(sqlparam);

            sqlparam = new MySqlParameter("@Tag", MySqlDbType.VarChar);
            sqlparam.Value = myData.Tag;
            listSqlParams.Add(sqlparam);

            sqlparam = new MySqlParameter("@Filename", MySqlDbType.VarChar);
            sqlparam.Value = myData.Filename;
            listSqlParams.Add(sqlparam);

            int HdKind = (myData.HdKind != null) ? myData.HdKind.Kind : 0;
            sqlparam = new MySqlParameter("@HdKind", MySqlDbType.Int16);
            sqlparam.Value = HdKind;
            listSqlParams.Add(sqlparam);

            sqlparam = new MySqlParameter("@MovieFilesId", MySqlDbType.Int32);
            sqlparam.Value = myData.FileId;
            listSqlParams.Add(sqlparam);

            sqlparam = new MySqlParameter("@Package", MySqlDbType.VarChar);
            sqlparam.Value = myData.Package;
            listSqlParams.Add(sqlparam);

            sqlparam = new MySqlParameter("@Thumbnail", MySqlDbType.VarChar);
            sqlparam.Value = myData.Thumbnail;
            listSqlParams.Add(sqlparam);

            sqlparam = new MySqlParameter("@DownloadFiles", MySqlDbType.VarChar);
            sqlparam.Value = myData.DownloadFiles;
            listSqlParams.Add(sqlparam);

            command.Parameters.AddRange(listSqlParams.ToArray());

            command.ExecuteNonQuery();

            MovieImportData data = GetNewest(dbcon);

            if (!data.ProductNumber.Equals(myData.ProductNumber))
                throw new Exception("最新のデータが違うため、取得出来ませんでした");

            return data;
        }

        public void DbUpdate(MovieImportData myData, MySqlDbConnection myDbCon)
        {
            MySqlDbConnection dbcon;
            string sqlcmd = "";

            // 引数にコネクションが指定されていた場合は指定されたコネクションを使用
            if (myDbCon != null)
                dbcon = myDbCon;
            else
                dbcon = new MySqlDbConnection();

            sqlcmd = "UPDATE import ";
            sqlcmd += "SET COPY_TEXT = @CopyText";
            sqlcmd += ", KIND = @Kind";
            sqlcmd += ", MATCH_PRODUCT = @MatchProduct";
            sqlcmd += ", PRODUCT_NUMBER = @ProductNumber";
            sqlcmd += ", SELL_DATE = @ProductDate";
            sqlcmd += ", MAKER = @Maker ";
            sqlcmd += ", TITLE = @Title ";
            sqlcmd += ", ACTRESSES = @Actresses ";
            sqlcmd += ", RAR_FLAG = @RarFlag ";
            sqlcmd += ", SPLIT_FLAG = @SplitFlag ";
            sqlcmd += ", NAME_ONLY_FLAG = @NameOnlyFlag ";
            sqlcmd += ", TAG = @Tag ";
            sqlcmd += ", FILENAME = @Filename ";
            sqlcmd += ", HD_KIND = @HdKind ";
            sqlcmd += ", movie_file_id = @MovieFilesId ";
            sqlcmd += ", rating = @Rating ";
            sqlcmd += ", package = @Package ";
            sqlcmd += ", thumbnail = @Thumbnail ";
            sqlcmd += ", download_files = @DownloadFiles ";
            sqlcmd += "WHERE ID = @Id ";

            MySqlCommand command = new MySqlCommand(sqlcmd, dbcon.getMySqlConnection());
            DataTable dtSaraly = new DataTable();

            List<MySqlParameter> listSqlParams = new List<MySqlParameter>();

            MySqlParameter sqlparam = new MySqlParameter("@CopyText", MySqlDbType.Text);
            sqlparam.Value = myData.CopyText;
            listSqlParams.Add(sqlparam);

            sqlparam = new MySqlParameter("@Kind", MySqlDbType.Int32);
            sqlparam.Value = myData.Kind;
            listSqlParams.Add(sqlparam);

            sqlparam = new MySqlParameter("@MatchProduct", MySqlDbType.VarChar);
            sqlparam.Value = myData.MatchProduct;
            listSqlParams.Add(sqlparam);

            sqlparam = new MySqlParameter("@ProductNumber", MySqlDbType.VarChar);
            sqlparam.Value = myData.ProductNumber;
            listSqlParams.Add(sqlparam);

            sqlparam = new MySqlParameter("@ProductDate", MySqlDbType.DateTime);
            sqlparam.Value = myData.ProductDate;
            listSqlParams.Add(sqlparam);

            sqlparam = new MySqlParameter("@Maker", MySqlDbType.Text);
            sqlparam.Value = myData.StrMaker;
            listSqlParams.Add(sqlparam);

            sqlparam = new MySqlParameter("@Title", MySqlDbType.Text);
            sqlparam.Value = myData.Title;
            listSqlParams.Add(sqlparam);

            sqlparam = new MySqlParameter("@Actresses", MySqlDbType.Text);
            sqlparam.Value = myData.Actresses;
            listSqlParams.Add(sqlparam);

            sqlparam = new MySqlParameter("@RarFlag", MySqlDbType.Int16);
            sqlparam.Value = myData.RarFlag;
            listSqlParams.Add(sqlparam);

            sqlparam = new MySqlParameter("@SplitFlag", MySqlDbType.Int16);
            sqlparam.Value = myData.SplitFlag;
            listSqlParams.Add(sqlparam);

            sqlparam = new MySqlParameter("@NameOnlyFlag", MySqlDbType.Int16);
            sqlparam.Value = myData.NameOnlyFlag;
            listSqlParams.Add(sqlparam);

            sqlparam = new MySqlParameter("@Tag", MySqlDbType.VarChar);
            sqlparam.Value = myData.Tag;
            listSqlParams.Add(sqlparam);

            sqlparam = new MySqlParameter("@Filename", MySqlDbType.Text);
            sqlparam.Value = myData.Filename;
            listSqlParams.Add(sqlparam);

            int HdKind = (myData.HdKind != null) ? myData.HdKind.Kind : ((myData.HdFlag == true) ? 1 : 0);
            sqlparam = new MySqlParameter("@HdKind", MySqlDbType.Int16);
            sqlparam.Value = HdKind;
            listSqlParams.Add(sqlparam);

            sqlparam = new MySqlParameter("@MovieFilesId", MySqlDbType.Int32);
            sqlparam.Value = myData.FileId;
            listSqlParams.Add(sqlparam);

            sqlparam = new MySqlParameter("@Rating", MySqlDbType.Int32);
            sqlparam.Value = myData.Rating;
            listSqlParams.Add(sqlparam);

            sqlparam = new MySqlParameter("@Package", MySqlDbType.VarChar);
            sqlparam.Value = myData.Package;
            listSqlParams.Add(sqlparam);

            sqlparam = new MySqlParameter("@Thumbnail", MySqlDbType.VarChar);
            sqlparam.Value = myData.Thumbnail;
            listSqlParams.Add(sqlparam);

            sqlparam = new MySqlParameter("@DownloadFiles", MySqlDbType.VarChar);
            sqlparam.Value = myData.DownloadFiles;
            listSqlParams.Add(sqlparam);

            sqlparam = new MySqlParameter("@Id", MySqlDbType.Int32);
            sqlparam.Value = myData.Id;
            listSqlParams.Add(sqlparam);

            dbcon.SetParameter(listSqlParams.ToArray());

            dbcon.execSqlCommand(sqlcmd);

            return;
        }

        public void DbDelete(MovieImportData myData, MySqlDbConnection myDbCon)
        {
            MySqlDbConnection dbcon;
            string sqlcmd = "";

            // 引数にコネクションが指定されていた場合は指定されたコネクションを使用
            if (myDbCon != null)
                dbcon = myDbCon;
            else
                dbcon = new MySqlDbConnection();

            dbcon.openConnection();

            sqlcmd = "DELETE FROM import WHERE ID = @Id ";

            MySqlCommand command = new MySqlCommand(sqlcmd, dbcon.getMySqlConnection());
            DataTable dtSaraly = new DataTable();

            List<MySqlParameter> listSqlParams = new List<MySqlParameter>();

            MySqlParameter sqlparam = new MySqlParameter("@Id", MySqlDbType.Int32);
            sqlparam.Value = myData.Id;
            listSqlParams.Add(sqlparam);

            dbcon.SetParameter(listSqlParams.ToArray());

            command.Parameters.Add(sqlparam);

            command.ExecuteNonQuery();

            return;
        }

        public MovieImportData GetNewest(MySqlDbConnection myDbCon)
        {
            MovieImportData newestData = new MovieImportData();

            MySqlDbConnection dbcon;
            string sqlcmd = "";

            // 引数にコネクションが指定されていた場合は指定されたコネクションを使用
            if (myDbCon != null)
                dbcon = myDbCon;
            else
                dbcon = new MySqlDbConnection();

            sqlcmd = "SELECT ID, COPY_TEXT, KIND, MATCH_PRODUCT, PRODUCT_NUMBER, SELL_DATE, MAKER, TITLE, ACTRESSES, RAR_FLAG, SPLIT_FLAG, NAME_ONLY_FLAG, TAG, FILENAME, RATING, JAV_POST_DATE, PACKAGE, THUMBNAIL, DOWNLOAD_FILES, jav_id, CREATED_AT, UPDATED_AT ";
            sqlcmd = sqlcmd + "FROM import ";
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
                    newestData.Id = MysqlExportCommon.GetDbInt(reader, 0);
                    newestData.CopyText = MysqlExportCommon.GetDbString(reader, 1);
                    newestData.Kind = MysqlExportCommon.GetDbInt(reader, 2);
                    newestData.MatchProduct = MysqlExportCommon.GetDbString(reader, 3);
                    newestData.ProductNumber = MysqlExportCommon.GetDbString(reader, 4);
                    newestData.ProductDate = MysqlExportCommon.GetDbDateTime(reader, 5);
                    newestData.StrMaker = MysqlExportCommon.GetDbString(reader, 6);
                    newestData.Title = MysqlExportCommon.GetDbString(reader, 7);
                    newestData.Actresses = MysqlExportCommon.GetDbString(reader, 8);
                    newestData.RarFlag = Convert.ToBoolean(MysqlExportCommon.GetDbInt(reader, 9));
                    newestData.SplitFlag = Convert.ToBoolean(MysqlExportCommon.GetDbInt(reader, 10));
                    newestData.NameOnlyFlag = Convert.ToBoolean(MysqlExportCommon.GetDbInt(reader, 11));
                    newestData.Tag = MysqlExportCommon.GetDbString(reader, 12);
                    newestData.Filename = MysqlExportCommon.GetDbString(reader, 13);
                    newestData.Rating = MysqlExportCommon.GetDbInt(reader, 14);
                    newestData.JavPostDate = MysqlExportCommon.GetDbDateTime(reader, 15);
                    newestData.Package = MysqlExportCommon.GetDbString(reader, 16);
                    newestData.Thumbnail = MysqlExportCommon.GetDbString(reader, 17);
                    newestData.DownloadFiles = MysqlExportCommon.GetDbString(reader, 18);
                    newestData.JavId = MysqlExportCommon.GetDbLong(reader, 19);
                    newestData.CreateDate = MysqlExportCommon.GetDbDateTime(reader, 20);
                    newestData.UpdateDate = MysqlExportCommon.GetDbDateTime(reader, 21);
                }
            }
            finally
            {
                reader.Close();
            }

            reader.Close();

            myDbCon.closeConnection();

            return newestData;
        }

        public List<MovieImportData> GetList(MySqlDbConnection myDbCon)
        {
            List<MovieImportData> listData = new List<MovieImportData>();

            MySqlDbConnection dbcon;
            string sqlcmd = "";

            // 引数にコネクションが指定されていた場合は指定されたコネクションを使用
            if (myDbCon != null)
                dbcon = myDbCon;
            else
                dbcon = new MySqlDbConnection();

            sqlcmd = "SELECT ID, copy_text, KIND, MATCH_PRODUCT, PRODUCT_NUMBER, sell_date, MAKER, TITLE, ACTRESSES, RAR_FLAG, SPLIT_FLAG, NAME_ONLY_FLAG, TAG, FILENAME, CREATED_AT, UPDATED_AT, HD_KIND, movie_file_id, RATING, JAV_POST_DATE, SIZE, PACKAGE, THUMBNAIL, DOWNLOAD_FILES, SEARCH_RESULT, DETAIL, jav_id ";
            sqlcmd = sqlcmd + "FROM import ";
            sqlcmd = sqlcmd + "ORDER BY JAV_POST_DATE ";

            MySqlDataReader reader = null;
            try
            {
                reader = dbcon.GetExecuteReader(sqlcmd);

                do
                {
                    if (reader.IsClosed)
                    {
                        //_logger.Debug("reader.IsClosed");
                        throw new Exception("MOVIE_SITESTOREの取得でreaderがクローズされています");
                    }

                    while (reader.Read())
                    {
                        MovieImportData data = new MovieImportData();

                        data.Id = MysqlExportCommon.GetDbInt(reader, 0);
                        data.CopyText = MysqlExportCommon.GetDbString(reader, 1);
                        data.Kind = MysqlExportCommon.GetDbInt(reader, 2);
                        data.MatchProduct = MysqlExportCommon.GetDbString(reader, 3);
                        data.ProductNumber = MysqlExportCommon.GetDbString(reader, 4);
                        data.ProductDate = MysqlExportCommon.GetDbDateTime(reader, 5);
                        data.StrMaker = MysqlExportCommon.GetDbString(reader, 6);
                        data.Title = MysqlExportCommon.GetDbString(reader, 7);
                        data.Actresses = MysqlExportCommon.GetDbString(reader, 8);
                        data.RarFlag = Convert.ToBoolean(MysqlExportCommon.GetDbInt(reader, 9));
                        data.SplitFlag = Convert.ToBoolean(MysqlExportCommon.GetDbInt(reader, 10));
                        data.NameOnlyFlag = Convert.ToBoolean(MysqlExportCommon.GetDbInt(reader, 11));
                        data.Tag = MysqlExportCommon.GetDbString(reader, 12);
                        data.Filename = MysqlExportCommon.GetDbString(reader, 13);
                        data.CreateDate = MysqlExportCommon.GetDbDateTime(reader, 14);
                        data.UpdateDate = MysqlExportCommon.GetDbDateTime(reader, 15);
                        data.SetHdKind(MysqlExportCommon.GetDbInt(reader, 16));
                        data.FileId = MysqlExportCommon.GetDbInt(reader, 17);
                        data.Rating = MysqlExportCommon.GetDbInt(reader, 18);
                        data.JavPostDate = MysqlExportCommon.GetDbDateTime(reader, 19);
                        data.Size = MysqlExportCommon.GetDbLong(reader, 20);
                        data.Package = MysqlExportCommon.GetDbString(reader, 21);
                        data.Thumbnail = MysqlExportCommon.GetDbString(reader, 22);
                        data.DownloadFiles = MysqlExportCommon.GetDbString(reader, 23);
                        data.SearchResult = MysqlExportCommon.GetDbString(reader, 24);
                        data.Detail = MysqlExportCommon.GetDbString(reader, 25);
                        data.JavId = MysqlExportCommon.GetDbLong(reader, 26);

                        listData.Add(data);
                    }
                } while (reader.NextResult());
            }
            finally
            {
                reader.Close();
            }

            myDbCon.closeConnection();

            return listData;
        }

    }
}
