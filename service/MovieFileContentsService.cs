﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using WpfScrapingArrangement.collection;

namespace WpfScrapingArrangement
{
    class MovieFileContentsService
    {
        public double TotalLength = 0;
        public int FileCount = 0;

        public List<MovieFileContents> GetDirFiles(string myPathname)
        {
            if (!Directory.Exists(myPathname))
                return null;

            // 1行でJPGファイルを除いたリストを取得できていたが、正常に動作しなくなったので、下のループlistFilesへのAddを使用
            //IEnumerable<string> files = from file in Directory.GetFiles(@strDir) where regex.IsMatch(file) select file;

            string[] files = Directory.GetFiles(myPathname);

            Regex regex = new Regex(FileGeneTargetFilesCollection.REGEX_MOVIE_EXTENTION);
            List<string> listFiles = new List<string>();

            foreach (var file in files)
            {
                FileInfo fileinfo = new FileInfo(file.ToString());
                if (regex.IsMatch(fileinfo.Name))
                {
                    TotalLength = TotalLength + fileinfo.Length;
                    FileCount++;
                    listFiles.Add(file.ToString());
                }
            }

            //var files = Directory.GetFiles(@"Z:\DVDRip", "*jpg;*avi", SearchOption.TopDirectoryOnly);
            List<MovieFileContents> listMContents = new List<MovieFileContents>();
            List<string> listManyFileCheck = new List<string>();

            foreach (var file in listFiles)
            {
                FileInfo fileinfo = new FileInfo(file.ToString());

                string ExtWithoutName = fileinfo.Name.Replace(fileinfo.Extension, "");

                bool isManyFileExist = false;
                foreach (string data in listManyFileCheck)
                {
                    if (ExtWithoutName.IndexOf(data) >= 0)
                    {
                        isManyFileExist = true;
                        break;
                    }
                }

                if (isManyFileExist)
                    continue;

                MovieFileContents fileData = new MovieFileContents();
                fileData.Name = ExtWithoutName;
                fileData.Extension = fileinfo.Extension.Replace(".", "").ToUpper();
                fileData.Label = myPathname;
                fileData.FileDate = fileinfo.LastWriteTime;
                fileData.FileCount = 1;
                fileData.Size = fileinfo.Length;

                fileData.Parse();

                // サムネイル画像を見るを追加する
                string FileExt = "";

                FileExt = fileinfo.Extension;

                if (FileExt.Length > 0)
                {
                    string ThumbnailFilename = fileinfo.FullName.Replace(FileExt, "") + "_th.jpg";

                    if (File.Exists(ThumbnailFilename))
                    {
                        fileData.IsExistsThumbnail = true;
                    }
                }

                Regex regexManyFile = new Regex("_[0-9]$");

                if (regexManyFile.IsMatch(ExtWithoutName))
                {
                    MovieFileContents data = new MovieFileContents();
                    string FilesPattern = Regex.Replace(fileinfo.Name, "_[0-9]\\" + fileinfo.Extension + "$", "*" + fileinfo.Extension);

                    string[] arrFiles = System.IO.Directory.GetFiles(fileinfo.Directory.FullName, FilesPattern, SearchOption.TopDirectoryOnly);

                    if (arrFiles != null && arrFiles.Length > 1)
                    {
                        listManyFileCheck.Add(Regex.Replace(ExtWithoutName, "_[0-9]$", ""));

                        long size = 0;
                        foreach (string pathname in arrFiles)
                        {
                            FileInfo fileinfoMany = new FileInfo(pathname);
                            size += fileinfoMany.Length;
                        }

                        fileData.Name = Regex.Replace(fileinfo.Name, "_[0-9]\\" + fileinfo.Extension + "$", "");
                        fileData.Size = size;
                        fileData.FileCount = arrFiles.Length;
                    }
                }

                listMContents.Add(fileData);
            }

            return listMContents;
        }

        public List<MovieFileContents> GetDbContents()
        {
            DbConnection dbcon = new DbConnection();

            List<MovieFileContents> listMContents = new List<MovieFileContents>();

            string queryString = "SELECT ID, NAME, SIZE, FILE_DATE, LABEL, SELL_DATE, PRODUCT_NUMBER, TAG, EXTENSION FROM MOVIE_FILES ORDER BY ID  ";

            dbcon.openConnection();

            SqlCommand command = new SqlCommand(queryString, dbcon.getSqlConnection());
            command.CommandTimeout = 120;

            //SqlParameter param = new SqlParameter("@pActressName", SqlDbType.VarChar);
            //param.Value = myGroupName;
            //command.Parameters.Add(param);

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
                    data.Tag = DbExportCommon.GetDbString(reader, 7);
                    data.Extension = DbExportCommon.GetDbString(reader, 8);

                    listMContents.Add(data);
                }
            } while (reader.NextResult());

            reader.Close();

            dbcon.closeConnection();

            return listMContents;
        }

        public void DbUpdateFileInfo(MovieFileContents myData, DbConnection myDbCon)
        {
            DbConnection dbcon;
            string sqlcmd = "";

            // 引数にコネクションが指定されていた場合は指定されたコネクションを使用
            if (myDbCon != null)
                dbcon = myDbCon;
            else
                dbcon = new DbConnection();

            //string queryString = "SELECT ID, NAME, SIZE, FILE_DATE, LABEL, SELL_DATE, PRODUCT_NUMBER FROM MOVIE_FILES ORDER BY FILE_DATE DESC ";
            sqlcmd = "UPDATE MOVIE_FILES ";
            sqlcmd += "SET ";
            sqlcmd += "  NAME = @Name ";
            sqlcmd += ", EXTENSION = @Extension ";
            sqlcmd += ", SIZE = @Size ";
            sqlcmd += ", FILE_DATE = @FileDate ";
            sqlcmd += "WHERE ID = @Id ";

            SqlCommand scmd = new SqlCommand(sqlcmd, dbcon.getSqlConnection());
            DataTable dtSaraly = new DataTable();

            List<SqlParameter> listSqlParams = new List<SqlParameter>();

            SqlParameter sqlparam = new SqlParameter("@Name", SqlDbType.VarChar);
            sqlparam.Value = myData.Name;
            listSqlParams.Add(sqlparam);

            sqlparam = new SqlParameter("@Extension", SqlDbType.VarChar);
            sqlparam.Value = myData.Extension;
            listSqlParams.Add(sqlparam);

            sqlparam = new SqlParameter("@Size", SqlDbType.BigInt);
            sqlparam.Value = myData.Size;
            listSqlParams.Add(sqlparam);

            sqlparam = new SqlParameter("@FileDate", SqlDbType.DateTime);
            sqlparam.Value = myData.FileDate;
            listSqlParams.Add(sqlparam);

            sqlparam = new SqlParameter("@Id", SqlDbType.Int);
            sqlparam.Value = myData.Id;
            listSqlParams.Add(sqlparam);

            dbcon.SetParameter(listSqlParams.ToArray());

            dbcon.execSqlCommand(sqlcmd);

            return;
        }

        public string GetFileLength()
        {
            double SizeTera = TotalLength / 1024 / 1024 / 1024;
            string SizeStr = String.Format("{0:###,###,###,###}", SizeTera);

            return SizeStr;
        }
    }
}
