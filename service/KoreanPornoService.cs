using Microsoft.VisualBasic.FileIO;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;
using WpfScrapingArrangement.collection;
using WpfScrapingArrangement.data;

namespace WpfScrapingArrangement.service
{
    class KoreanPornoService
    {
        //public static string Path = @"C:\Users\充一\Desktop\Downloads\TEMP";
        public string BasePath;

        public string ExportPath;

        private bool IsCheckMoveFolder;

        public KoreanPornoService(string myPath, string myExportPath, bool? myIsCheckMoveFolder)
        {
            BasePath = myPath;
            ExportPath = myExportPath;

            if (myIsCheckMoveFolder != null)
                IsCheckMoveFolder = Convert.ToBoolean(myIsCheckMoveFolder);
            else
                IsCheckMoveFolder = false;
        }

        public List<KoreanPornoFileInfo> GetFileInfo(string myBaseFilename, DateTime myChangeLastWriteTime, string myArchiveName)
        {
            List<KoreanPornoFileInfo> listFiles = new List<KoreanPornoFileInfo>();

            string pathname = System.IO.Path.Combine(BasePath, myArchiveName);

            //if (!Directory.Exists(pathname) && !File.Exists(pathname))
            //    return null;

            string[] archiveMovieFiles = null;
            bool isExist = false;
            Regex regexMovie = new Regex(FileGeneTargetFilesCollection.REGEX_MOVIEONLY_EXTENTION, RegexOptions.IgnoreCase);
            string oneFilename = "";
            if (!Directory.Exists(pathname))
            {
                archiveMovieFiles = Directory.GetFiles(BasePath, myArchiveName + "*");
                if (archiveMovieFiles.Length > 0)
                    foreach (string file in archiveMovieFiles)
                    {
                        if (regexMovie.IsMatch(file))
                        {
                            oneFilename = file;
                            isExist = true;
                            break;
                        }
                    }

                if (File.Exists(pathname))
                    isExist = true;

            }
            else
                isExist = true;

            if (isExist == false)
                return null;

            Regex regexImage = new Regex(FileGeneTargetFilesCollection.REGEX_IMAGE_EXTENTION, RegexOptions.IgnoreCase);

            List<string> jpegFileList = new List<string>();

            string[] archiveFiles = Directory.GetFiles(BasePath, myArchiveName + "*", System.IO.SearchOption.TopDirectoryOnly);
            foreach (string file in archiveFiles)
            {
                if (regexImage.IsMatch(file))
                    jpegFileList.Add(file);
            }

            string[] jpegFiles = jpegFileList.ToArray();

            int fileNo = 1;
            foreach (string file in jpegFiles)
            {
                KoreanPornoFileInfo koreanFile = new KoreanPornoFileInfo(file);

                string suffix = "";
                if (jpegFiles.Length > 1)
                    suffix = "_" + fileNo;

                koreanFile.IsSelected = true;

                koreanFile.DisplayFilename = "XXX" + suffix + koreanFile.FileInfo.Extension.ToLower();
                koreanFile.ChangeFilename = myBaseFilename + suffix + koreanFile.FileInfo.Extension.ToLower();

                koreanFile.ChangeLastWriteTime = myChangeLastWriteTime;

                fileNo++;

                listFiles.Add(koreanFile);
            }

            string[] files = null;

            if (Directory.Exists(pathname))
                files = Directory.GetFiles(pathname, "*", System.IO.SearchOption.AllDirectories);
            else
            {
                files = new string[1];
                if (oneFilename.Length > 0)
                    files[0] = oneFilename;
                else
                    files[0] = pathname;
            }

            Regex regex = new Regex(FileGeneTargetFilesCollection.REGEX_MOVIE_EXTENTION, RegexOptions.IgnoreCase);

            List<KoreanPornoFileInfo> listKFiles = new List<KoreanPornoFileInfo>();
            int fileCnt = 0;
            foreach (string file in files)
            {
                KoreanPornoFileInfo koreanFile = new KoreanPornoFileInfo(file);

                if (regex.IsMatch(koreanFile.FileInfo.Name))
                    fileCnt++;
            }

            fileNo = 1;
            foreach (string file in files)
            {
                KoreanPornoFileInfo koreanFile = new KoreanPornoFileInfo(file);

                Match match = regex.Match(koreanFile.FileInfo.Name);
                if (match.Success)
                {
                    koreanFile.IsSelected = true;

                    string suffix = "";
                    if (fileCnt > 1)
                        suffix = "_" + fileNo;

                    koreanFile.DisplayChangeFilename = "名前" + suffix + koreanFile.FileInfo.Extension.ToLower();
                    koreanFile.ChangeFilename = myBaseFilename + suffix + koreanFile.FileInfo.Extension.ToLower();

                    koreanFile.ChangeLastWriteTime = myChangeLastWriteTime;

                    fileNo++;
                }
                listFiles.Add(koreanFile);
            }


            return listFiles;
        }

        /// <summary>
        /// アーカイブの中の選択された対象ファイルを取得
        /// </summary>
        private List<KoreanPornoFileInfo> GetTargetFiles(List<KoreanPornoFileInfo> myListFileInfo)
        {
            List<KoreanPornoFileInfo> selFiles = new List<KoreanPornoFileInfo>();

            long selSize = 0;
            foreach (KoreanPornoFileInfo data in myListFileInfo)
            {
                if (data.IsSelected)
                {
                    selFiles.Add(data);
                    selSize += data.FileInfo.Length;
                }
            }

            return selFiles;
        }

        public void ExecuteArrangement(MovieImportData myTargetImportData, StoreData myStoreData, List<KoreanPornoFileInfo> myListFileInfo)
        {
            List<KoreanPornoFileInfo> jpegFiles = new List<KoreanPornoFileInfo>();
            List<KoreanPornoFileInfo> movieFiles = new List<KoreanPornoFileInfo>();

            Regex regex = new Regex(FileGeneTargetFilesCollection.REGEX_IMAGE_EXTENTION, RegexOptions.IgnoreCase);
            foreach (KoreanPornoFileInfo data in myListFileInfo)
            {
                if (data.IsSelected && regex.IsMatch(data.FileInfo.Name))
                    jpegFiles.Add(data);
                else if (data.IsSelected)
                    movieFiles.Add(data);
            }

            long totalSize = 0;
            string extensions = "";
            foreach (KoreanPornoFileInfo data in movieFiles)
            {
                totalSize += data.FileInfo.Length;

                string ext = data.FileInfo.Extension.Substring(1);
                if (extensions.Length <= 0)
                    extensions = ext;
                else
                {
                    if (extensions.IndexOf(ext, StringComparison.OrdinalIgnoreCase) < 0)
                        extensions = extensions + "," + ext;
                }
            }

            KoreanPornoData targetData = new KoreanPornoData();
            targetData.Name = myTargetImportData.CopyText;
            targetData.Size = totalSize;
            targetData.FileCount = movieFiles.Count;
            targetData.Extension = extensions.ToUpper();
            targetData.Label = ExportPath;
            targetData.LastWriteTime = myTargetImportData.JavPostDate;
            targetData.Tag = myTargetImportData.Tag;

            DbExportContents(targetData, myStoreData);
            DbExport(targetData, new DbConnection());

            // ファイル移動先の生成（D:\Downloads\TEMP\KOREAN_PORNO7のフォルダを無ければ作成）
            string moveDestName = new DirectoryInfo(ExportPath).Name;
            string moveDestPath = Path.Combine(BasePath, moveDestName);

            if (!Directory.Exists(moveDestPath))
            {
                DirectoryInfo dir = Directory.CreateDirectory(moveDestPath);
                moveDestPath = dir.FullName;
            }

            // JPEGファイルsの移動
            foreach (var jpegFile in jpegFiles)
                File.Move(jpegFile.FileInfo.FullName, Path.Combine(moveDestPath, jpegFile.ChangeFilename));

            // フォルダ作成しての動画の移動はJPEGの移動が終了してから
            if (IsCheckMoveFolder == true)
            {
                moveDestPath = Path.Combine(moveDestPath, targetData.Name);

                if (!Directory.Exists(moveDestPath))
                {
                    DirectoryInfo dir = Directory.CreateDirectory(moveDestPath);
                    moveDestPath = dir.FullName;
                }
            }

            // 動画ファイルの移動、ファイル更新日
            foreach (KoreanPornoFileInfo file in movieFiles)
            {
                string filename = "";
                if (IsCheckMoveFolder == true)
                    filename = file.DisplayFilename;
                else
                    filename = file.ChangeFilename;

                string destFilename = System.IO.Path.Combine(moveDestPath, filename);
                // File.SetLastWriteTime(file.FileInfo.FullName, file.ChangeLastWriteTime);
                File.Move(file.FileInfo.FullName, destFilename);
            }

            if (IsCheckMoveFolder == true)
            {
                foreach (KoreanPornoFileInfo file in myListFileInfo)
                {
                    if (file.FileInfo.Extension == ".txt")
                        continue;

                    if (file.FileInfo.Extension == ".zip")
                        continue;

                    if (!File.Exists(file.FileInfo.FullName))
                        continue;

                    string destFilename = System.IO.Path.Combine(moveDestPath, file.DisplayFilename);
                    File.Move(file.FileInfo.FullName, destFilename);
                }
            }

            string rarFile = myTargetImportData.ProductNumber + ".rar";
            string archiveFilePath = System.IO.Path.Combine(BasePath, rarFile);
            string frozenFolderPath = System.IO.Path.Combine(BasePath, myTargetImportData.ProductNumber);

            // 圧縮ファイル(Rar)の削除（ゴミ箱）
            if (File.Exists(archiveFilePath))
            {
                FileSystem.DeleteFile(
                    archiveFilePath,
                    UIOption.OnlyErrorDialogs,
                    RecycleOption.SendToRecycleBin);
            }

            // 解凍フォルダの削除（ゴミ箱）
            if (!string.IsNullOrEmpty(frozenFolderPath))
            {
                if (Directory.Exists(frozenFolderPath))
                {
                    FileSystem.DeleteDirectory(frozenFolderPath,
                        UIOption.OnlyErrorDialogs,
                        RecycleOption.SendToRecycleBin);
                }
            }

            MovieImportService importService = new MovieImportService();
            importService.DbDelete(myTargetImportData, new MySqlDbConnection());
        }

        private void DbExport(KoreanPornoData myData, DbConnection myDbCon)
        {
            string sqlCommand = "INSERT INTO MOVIE_FILES (NAME, SIZE, FILE_DATE, LABEL, FILE_COUNT, EXTENSION, TAG) VALUES( @pName, @pSize, @pFileDate, @pLabel, @pFileCount, @pExtension, @pTag )";

            SqlCommand command = new SqlCommand(sqlCommand, myDbCon.getSqlConnection());

            List<SqlParameter> sqlparamList = new List<SqlParameter>();
            SqlParameter sqlparam = new SqlParameter();

            sqlparam = new SqlParameter("@pName", SqlDbType.VarChar);
            sqlparam.Value = myData.Name;
            sqlparamList.Add(sqlparam);

            sqlparam = new SqlParameter("@pSize", SqlDbType.Decimal);
            sqlparam.Value = myData.Size;
            sqlparamList.Add(sqlparam);

            sqlparam = new SqlParameter("@pFileDate", SqlDbType.DateTime);
            sqlparam.Value = myData.LastWriteTime;
            sqlparamList.Add(sqlparam);

            sqlparam = new SqlParameter("@pLabel", SqlDbType.VarChar);
            sqlparam.Value = myData.Label.ToUpper();
            sqlparamList.Add(sqlparam);

            sqlparam = new SqlParameter("@pFileCount", SqlDbType.Int);
            sqlparam.Value = myData.FileCount;
            sqlparamList.Add(sqlparam);

            sqlparam = new SqlParameter("@pExtension", SqlDbType.VarChar);
            sqlparam.Value = myData.Extension.ToUpper();
            sqlparamList.Add(sqlparam);

            sqlparam = new SqlParameter("@pTag", SqlDbType.VarChar);
            sqlparam.Value = myData.Tag.ToUpper();
            sqlparamList.Add(sqlparam);

            myDbCon.SetParameter(sqlparamList.ToArray());
            myDbCon.execSqlCommand(sqlCommand);

            return;
        }

        public void DbExportContents(KoreanPornoData myData, StoreData myStoreData)
        {
            MySqlDbConnection dbcon = new MySqlDbConnection();

            // データベースへ登録
            string sqlCommand = "INSERT INTO av.contents (store_label, name, extension, tag, file_date, file_count, size, file_status) "
                + "VALUES( @pStoreLabel, @pName, @pExtension, @pTag, @pFileDate, @pFileCount, @pSize, @pFileStatus )";

            MySqlCommand command = new MySqlCommand(sqlCommand, dbcon.getMySqlConnection());

            List<MySqlParameter> listParams = new List<MySqlParameter>();

            MySqlParameter param = new MySqlParameter("@pStoreLabel", MySqlDbType.VarChar);
            param.Value = myStoreData.Label;
            listParams.Add(param);

            param = new MySqlParameter("@pName", MySqlDbType.VarChar);
            param.Value = myData.Name;
            listParams.Add(param);

            param = new MySqlParameter("@pExtension", MySqlDbType.VarChar);
            param.Value = myData.Extension;
            listParams.Add(param);

            param = new MySqlParameter("@pTag", MySqlDbType.VarChar);
            param.Value = myData.Tag;
            listParams.Add(param);

            param = new MySqlParameter("@pFileDate", MySqlDbType.DateTime);
            if (myData.LastWriteTime.Year >= 2000)
                param.Value = myData.LastWriteTime;
            else
                param.Value = Convert.DBNull;
            listParams.Add(param);

            param = new MySqlParameter("@pFileCount", MySqlDbType.Int32);
            param.Value = myData.FileCount;
            listParams.Add(param);

            param = new MySqlParameter("@pSize", MySqlDbType.Decimal);
            param.Value = myData.Size;
            listParams.Add(param);

            param = new MySqlParameter("@pFileStatus", MySqlDbType.VarChar);
            param.Value = "exist";
            listParams.Add(param);

            dbcon.SetParameter(listParams.ToArray());
            dbcon.execSqlCommand(sqlCommand);
        }

    }
}
