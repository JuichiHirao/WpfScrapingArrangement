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
