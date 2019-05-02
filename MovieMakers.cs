using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Diagnostics;
using MySql.Data.MySqlClient;

namespace WpfScrapingArrangement
{
    class MovieMakers
    {
        public static MovieMaker GetSearchByProductNumber(string myProductNumber)
        {
            DbConnection dbcon = new DbConnection();

            if (myProductNumber == null)
                myProductNumber = "";

            string[] product = myProductNumber.Split('-');
            List<MovieFileContents> listMContents = new List<MovieFileContents>();

            string queryString = "SELECT ID, NAME, SIZE, FILE_DATE, LABEL, SELL_DATE, PRODUCT_NUMBER FROM MOVIE_FILES WHERE PRODUCT_NUMBER LIKE @pProduct ORDER BY FILE_DATE DESC ";

            dbcon.openConnection();

            SqlCommand command = new SqlCommand(queryString, dbcon.getSqlConnection());

            SqlParameter param = new SqlParameter("@pProduct", SqlDbType.VarChar);
            param.Value = product[0] + "-%";
            command.Parameters.Add(param);

            SqlDataReader reader = command.ExecuteReader();

            do
            {
                while (reader.Read())
                {
                    MovieFileContents data = new MovieFileContents();

                    data.Id = DbExportCommon.GetDbInt(reader, 0);
                    data.Name = DbExportCommon.GetDbString(reader, 1);
                    data.Size = DbExportCommon.GetLong(reader, 2);
                    data.FileDate = DbExportCommon.GetDbDateTime(reader, 3);
                    data.Label = DbExportCommon.GetDbString(reader, 4);
                    data.SellDate = DbExportCommon.GetDbDateTime(reader, 5);
                    data.ProductNumber = DbExportCommon.GetDbString(reader, 6);

                    listMContents.Add(data);
                }
            } while (reader.NextResult());

            reader.Close();

            dbcon.closeConnection();

            MovieMaker maker = null;
            foreach(MovieFileContents data in listMContents)
            {
                maker = new MovieMaker();
                maker.ParseFromFileName(data);
                break;
            }

            return maker;
        }

        public static List<MovieMaker> GetAllData()
        {
            MySqlDbConnection dbcon = new MySqlDbConnection();

            List<MovieMaker> listMMakers = new List<MovieMaker>();

            string queryString = "SELECT ID, NAME, LABEL, KIND, MATCH_STR, MATCH_PRODUCT_NUMBER, REGISTERED_BY, CREATED_AT, UPDATED_AT FROM maker ORDER BY NAME DESC ";

            dbcon.openConnection();

            MySqlCommand command = new MySqlCommand(queryString, dbcon.getMySqlConnection());

            MySqlDataReader reader = command.ExecuteReader();

            do
            {
                while (reader.Read())
                {
                    MovieMaker data = new MovieMaker();

                    int colNo = 0;
                    data.Id = MysqlExportCommon.GetDbInt(reader, colNo++);
                    data.Name = MysqlExportCommon.GetDbString(reader, colNo++);
                    data.Label = MysqlExportCommon.GetDbString(reader, colNo++);
                    data.Kind = MysqlExportCommon.GetDbInt(reader, colNo++);
                    data.MatchStr = MysqlExportCommon.GetDbString(reader, colNo++);
                    data.MatchProductNumber = MysqlExportCommon.GetDbString(reader, colNo++);
                    data.RegisterdBy = MysqlExportCommon.GetDbString(reader, colNo++);
                    data.CreateDate = MysqlExportCommon.GetDbDateTime(reader, colNo++);
                    data.UpdateDate = MysqlExportCommon.GetDbDateTime(reader, colNo++);

                    listMMakers.Add(data);
                }
            } while (reader.NextResult());

            reader.Close();

            dbcon.closeConnection();

            return listMMakers;
        }
    }
}
