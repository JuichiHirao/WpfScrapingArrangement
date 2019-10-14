using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;

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

            if (!Directory.Exists(pathname) && !File.Exists(pathname))
                return null;

            string[] jpegFiles = Directory.GetFiles(BasePath, myArchiveName + "*jpg", System.IO.SearchOption.TopDirectoryOnly);

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
                files[0] = pathname;
            }

            Regex regex = new Regex(MovieFileContents.REGEX_MOVIE_EXTENTION, RegexOptions.IgnoreCase);

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

        public void ExecuteArrangement(MovieImportData myTargetImportData, List<KoreanPornoFileInfo> myListFileInfo)
        {
            List<KoreanPornoFileInfo> jpegFiles = new List<KoreanPornoFileInfo>();
            List<KoreanPornoFileInfo> movieFiles = new List<KoreanPornoFileInfo>();

            foreach (KoreanPornoFileInfo data in myListFileInfo)
            {
                if (data.IsSelected && data.FileInfo.Extension == ".jpg")
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

    }
}
