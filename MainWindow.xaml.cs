﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using Microsoft.VisualBasic.FileIO;
using System.ComponentModel;
using System.Globalization;
using System.Net;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using Codeplex.Data;
using WpfScrapingArrangement.service;
using WpfScrapingArrangement.collection;
using WpfScrapingArrangement.data;

namespace WpfScrapingArrangement
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public readonly static RoutedCommand PasteDateCopy = new RoutedCommand("PasteDateCopy", typeof(MainWindow));
        public readonly static RoutedCommand ChangeModeNormalRar = new RoutedCommand("ChangeModeNormalRar", typeof(MainWindow));
        public readonly static RoutedCommand ChangeModeNormalMovie = new RoutedCommand("ChangeModeNormalMovie", typeof(MainWindow));
        public readonly static RoutedCommand ChangeModeDateCopy = new RoutedCommand("ChangeModeDateCopy", typeof(MainWindow));
        public readonly static RoutedCommand CahngeModeFilenameGenerate = new RoutedCommand("CahngeModeFilenameGenerate", typeof(MainWindow));
        public readonly static RoutedCommand CahngeModeKoreanPorno = new RoutedCommand("CahngeModeKoreanPorno", typeof(MainWindow));

        private MovieFileContentsCollection ColViewMovieFileContents;

        private MovieImportCollection ColViewMovieImport;
        private FileGeneTargetFilesCollection ColViewFileGeneTargetFiles;
        private FileGeneTargetFilesCollection ColViewArrangementTarget; // dgridArrangementTarget
        private FileGeneTargetFilesCollection ColViewDestFiles; // dgridDestFile
        private MakerCollection ColViewMaker;
        private KoreanPornoCollection ColViewKoreanPorno;
        private StoreCollection ColViewStore;

        private List<ReplaceInfoData> dispctrlListReplaceInfoActress;

        private List<MovieMaker> dispinfoSelectDataGridMakers = null;
        private MovieImportData dispinfoSelectDataGridKoreanPorno = null;
        private MovieImportData dispinfoSelectMovieImportData= null;
        // 日付コピー時には各DataGridがColViewではなくなるので、戻すためのフラグ
        private bool dispinfoIsDateCopyPasteExecute = false;

        SettingXmlControl settingControl = null;
        Setting setting = null;
        ViewModel ViewData;
        bool isSelectSameMaker = false;

        public int dispctrlMode = 0;
        public const int MODE_NORMALRAR = 1;
        public const int MODE_NORMALMOVIE = 2;
        public const int MODE_DATECOPY = 3;
        public const int MODE_FILENAMEGENERATE = 4;
        public const int MODE_KOREANPORNO = 5;

        public const string JpegStorePath = @"C:\mydata\jav-save";

        public class SearchResultData
        {
            public SearchResultData(string myName, string myUrl)
            {
                Name = myName;
                Url = myUrl;
            }

            public string Name { get; set; }

            public string Url { get; set; }
        }

        public class ViewModel : INotifyPropertyChanged
        {
            private string _basePath;

            public string BasePath
            {
                get { return this._basePath; }
                set
                {
                    this._basePath = value;
                    var handler = this.PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("BasePath"));
                    }
                }
            }

            private string _labelPath;

            public string LabelPath
            {
                get { return this._labelPath; }
                set
                {
                    this._labelPath = value;
                    var handler = this.PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("LabelPath"));
                    }
                }
            }

            private string _koreanPornoPath;

            public string KoreanPornoPath
            {
                get { return this._koreanPornoPath; }
                set
                {
                    this._koreanPornoPath = value;
                    var handler = this.PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("KoreanPornoPath"));
                    }
                }
            }

            private string _koreanPornoExportPath;

            public string KoreanPornoExportPath
            {
                get { return this._koreanPornoExportPath; }
                set
                {
                    this._koreanPornoExportPath = value;
                    var handler = this.PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("KoreanPornoExportPath"));
                    }
                }
            }

            private string _koreanPornoExportStorePath;

            public string KoreanPornoExportStorePath
            {
                get { return this._koreanPornoExportStorePath; }
                set
                {
                    this._koreanPornoExportStorePath = value;
                    var handler = this.PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("KoreanPornoExportStorePath"));
                    }
                }
            }

            private string _FilenameGenDate;

            public string FilenameGenDate
            {
                get { return this._FilenameGenDate; }
                set
                {
                    this._FilenameGenDate = value;
                    var handler = this.PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("FilenameGenDate"));
                    }
                }
            }

            private string _ChangeFileName;

            public string ChangeFileName
            {
                get { return this._ChangeFileName; }
                set
                {
                    this._ChangeFileName = value;
                    var handler = this.PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("ChangeFileName"));
                    }
                }
            }

            private string _GenerateTargetFileName;

            public string GenerateTargetFileName
            {
                get { return this._GenerateTargetFileName; }
                set
                {
                    this._GenerateTargetFileName = value;
                    var handler = this.PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("GenerateTargetFileName"));
                    }
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
        }

        public MainWindow()
        {
            InitializeComponent();

            ViewData = new ViewModel { BasePath = "", FilenameGenDate = "", KoreanPornoExportPath = "", ChangeFileName = "", GenerateTargetFileName = "" };
            this.DataContext = ViewData;

            CommandBindings.Add(new CommandBinding(ChangeModeNormalRar, (s, ea) => { ChangeModeNormalRarExecute(s, ea); }, (s, ea) => ea.CanExecute = true));
            CommandBindings.Add(new CommandBinding(ChangeModeNormalMovie, (s, ea) => { ChangeModeNormalMovieExecute(s, ea); }, (s, ea) => ea.CanExecute = true));
            CommandBindings.Add(new CommandBinding(ChangeModeDateCopy, (s, ea) => { ChangeModeDateCopyExecute(s, ea); }, (s, ea) => ea.CanExecute = true));
            CommandBindings.Add(new CommandBinding(CahngeModeFilenameGenerate, (s, ea) => { CahngeModeFilenameGenerateExecute(s, ea); }, (s, ea) => ea.CanExecute = true));
            CommandBindings.Add(new CommandBinding(CahngeModeKoreanPorno, (s, ea) => { CahngeModeKoreanPornoExecute(s, ea); }, (s, ea) => ea.CanExecute = true));
            CommandBindings.Add(new CommandBinding(PasteDateCopy, (s, ea) => { PasteDateCopyExecute(s, ea); }, (s, ea) => ea.CanExecute = true));

            dispctrlMode = MODE_NORMALMOVIE;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            settingControl = new SettingXmlControl();
            setting = settingControl.GetData();

            if (setting.BasePath == null)
            {
                MessageBox.Show("SETTING.xmlが存在しないか、BasePathが設定されていません");
                return;
            }
            txtBasePath.Text = setting.BasePath;
            txtLabelPath.Text = setting.LabelPath;
            txtKoreanPornoPath.Text = setting.KoreanPornoPath;
            txtKoreanPornoExportPath.Text = setting.KoreanPornoExportPath;
            txtFilenameGenDate.Text = "";

            CahngeModeFilenameGenerateExecute(null, null);
            //ChangeModeNormalMovieExecute(null, null);

            OnGridTargetDisplay(null, null);

            ColViewFileGeneTargetFiles = new collection.FileGeneTargetFilesCollection(txtBasePath.Text);
            ColViewArrangementTarget = new collection.FileGeneTargetFilesCollection(txtBasePath.Text);
            ColViewDestFiles = new collection.FileGeneTargetFilesCollection(txtBasePath.Text);
            ColViewKoreanPorno = new collection.KoreanPornoCollection(txtKoreanPornoPath.Text, txtKoreanPornoExportPath.Text);
            ColViewMovieImport = new MovieImportCollection();
            ColViewStore = new StoreCollection();

            ColViewMovieFileContents = new collection.MovieFileContentsCollection();

            ColViewMaker = new collection.MakerCollection();

            //dgridCheckExistFiles.ItemsSource = ColViewFileGeneTargetFiles.ColViewListTargetFiles;
            dgridArrangementTarget.ItemsSource = ColViewArrangementTarget.ColViewListTargetFiles;
            dgridDestFile.ItemsSource = ColViewDestFiles.ColViewListTargetFiles;
            dgridKoreanPorno.ItemsSource = ColViewKoreanPorno.ColViewListData;
            dgridSelectTargetFilename.ItemsSource = ColViewMovieImport.collection;

            txtStatusBar.Width = statusbarMain.ActualWidth;
            txtStatusBar.Background = statusbarMain.Background;

            dgridSelectTargetFilename.Width = statusbarMain.ActualWidth;
            ColViewMovieImport.Refresh();

            DbConnection localDbCon = new DbConnection();
            txtbDbNowDate.Text = localDbCon.getDateStringSql("SELECT GETDATE()");

            ReplaceInfoService serviceReplaceInfo = new ReplaceInfoService();
            dispctrlListReplaceInfoActress = serviceReplaceInfo.GetTypeAll(data.ReplaceInfoData.EnumType.actress, new MySqlDbConnection());

            AvContentsService serviceContent = new AvContentsService();
            StoreData storeData = ColViewStore.GetMatchByPath(txtLabelPath.Text);
            long totalSize = serviceContent.GetStoreLabelTotalSize(storeData.Label, new MySqlDbConnection());
            long size = totalSize / 1024 / 1024 / 1024;
            string DisplaySize = "";
            if (size > 1023)
                DisplaySize = String.Format("{0}G", Convert.ToInt32(size));
            else
            {
                size = totalSize / 1024 / 1024 / 1024;
                DisplaySize = String.Format("{0:##.##}T", Convert.ToDouble(size));
            }
            txtbTotalSize.Text = DisplaySize;
        }

        /// <summary>
        /// 日付コピーを実行するとDataGridがColViewではなくなるので、日付コピーが終了した後は
        /// DataGridへの紐付けをColViewへ戻す
        /// </summary>
        private void OnModeChangeDataGrid()
        {
            if (dispinfoIsDateCopyPasteExecute)
            {
                dgridArrangementTarget.ItemsSource = ColViewArrangementTarget.ColViewListTargetFiles;
                dgridDestFile.ItemsSource = ColViewDestFiles.ColViewListTargetFiles;

                dispinfoIsDateCopyPasteExecute = false;
            }
        }

        public void ChangeModeNormalRarExecute(object sender, RoutedEventArgs e)
        {
            dispctrlMode = MODE_NORMALRAR;

            OnModeChangeDataGrid();

            ChangeModeNormal(null, null);
        }

        public void ChangeModeNormalMovieExecute(object sender, RoutedEventArgs e)
        {
            dispctrlMode = MODE_NORMALMOVIE;

            OnModeChangeDataGrid();

            ChangeModeNormal(null, null);
        }

        public void ChangeModeNormal(object sender, RoutedEventArgs e)
        {
            lgridMain.Visibility = System.Windows.Visibility.Visible;

            wpanelNormal.Visibility = System.Windows.Visibility.Visible;
            lgridNormalChangeFilename.Visibility = System.Windows.Visibility.Visible;

            lgridDateCopySource.Visibility = System.Windows.Visibility.Collapsed;
            lgridDateCopyDestination.Visibility = System.Windows.Visibility.Collapsed;

            lgridFilenameGenerate.Visibility = System.Windows.Visibility.Collapsed;

            lgridKoreanPornoArrange.Visibility = System.Windows.Visibility.Collapsed;

            OnGridTargetDisplay(null, null);
        }

        public void ChangeModeDateCopyExecute(object sender, RoutedEventArgs e)
        {
            dispctrlMode = MODE_DATECOPY;

            lgridMain.Visibility = System.Windows.Visibility.Visible;

            wpanelNormal.Visibility = System.Windows.Visibility.Collapsed;
            lgridNormalChangeFilename.Visibility = System.Windows.Visibility.Collapsed;

            lgridDateCopySource.Visibility = System.Windows.Visibility.Visible;
            lgridDateCopyDestination.Visibility = System.Windows.Visibility.Visible;

            lgridFilenameGenerate.Visibility = System.Windows.Visibility.Collapsed;

            ColViewArrangementTarget.Clear();
            ColViewDestFiles.Clear();
            //dgridDestFile.ItemsSource = null;
        }

        public void CahngeModeFilenameGenerateExecute(object sender, RoutedEventArgs e)
        {
            dispctrlMode = MODE_FILENAMEGENERATE;

            OnModeChangeDataGrid();

            lgridMain.Visibility = System.Windows.Visibility.Collapsed;
            wpanelNormal.Visibility = System.Windows.Visibility.Collapsed;
            lgridNormalChangeFilename.Visibility = System.Windows.Visibility.Collapsed;

            lgridDateCopySource.Visibility = System.Windows.Visibility.Collapsed;
            lgridDateCopyDestination.Visibility = System.Windows.Visibility.Collapsed;

            lgridFilenameGenerate.Width = this.ActualWidth - 30;
            lgridFilenameGenerate.Visibility = System.Windows.Visibility.Visible;

            lgridKoreanPornoArrange.Visibility = System.Windows.Visibility.Collapsed;
        }

        public void CahngeModeKoreanPornoExecute(object sender, RoutedEventArgs e)
        {
            dispctrlMode = MODE_KOREANPORNO;

            OnModeChangeDataGrid();

            lgridMain.Visibility = System.Windows.Visibility.Collapsed;
            wpanelNormal.Visibility = System.Windows.Visibility.Collapsed;
            lgridNormalChangeFilename.Visibility = System.Windows.Visibility.Collapsed;

            lgridDateCopySource.Visibility = System.Windows.Visibility.Collapsed;
            lgridDateCopyDestination.Visibility = System.Windows.Visibility.Collapsed;

            lgridFilenameGenerate.Visibility = System.Windows.Visibility.Collapsed;

            lgridKoreanPornoArrange.Visibility = System.Windows.Visibility.Visible;
        }

        private void btnDateCopyPasteSource_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.IDataObject data = Clipboard.GetDataObject();

            List<TargetFiles> files = GetClipbardFiles(data);

            if (files.Count > 0)
                dgridArrangementTarget.ItemsSource = files;

            dispinfoIsDateCopyPasteExecute = true;

            return;
        }

        private void btnPasteDestination_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.IDataObject data = Clipboard.GetDataObject();

            List<TargetFiles> files = GetClipbardFiles(data);

            if (files.Count > 0)
                dgridDestFile.ItemsSource = files;

            dispinfoIsDateCopyPasteExecute = true;

            return;
        }

        public void PasteDateCopyExecute(object sender, RoutedEventArgs e)
        {
            System.Windows.IDataObject data = Clipboard.GetDataObject();

            if (dispctrlMode == MODE_DATECOPY)
            {
                List<TargetFiles> files = GetClipbardFiles(data);

                if (files.Count > 0 && dgridArrangementTarget.ItemsSource == null)
                    dgridArrangementTarget.ItemsSource = files;
                else if (files.Count > 0)
                    dgridDestFile.ItemsSource = files;
            }
            else if (dispctrlMode == MODE_FILENAMEGENERATE)
            {
                btnPasteTitleText_Click(null, null);
            }

            return;
        }

        public List<TargetFiles> GetClipbardFiles(System.Windows.IDataObject myData)
        {
            List<TargetFiles> listPasteFile = new List<TargetFiles>();

            try
            {
                if (myData.GetDataPresent(DataFormats.Text))
                {
                    string ClipboardText = (string)myData.GetData(DataFormats.Text);
                    // クリップボードのテキストを改行毎に配列に設定
                    string[] ClipBoardList = ClipboardText.Split('\n');

                    foreach (string file in ClipBoardList)
                    {
                        FileInfo fileinfo = new FileInfo(file);

                        TargetFiles targetfiles = new TargetFiles();
                        targetfiles.FileInfo = fileinfo;
                        targetfiles.ListUpdateDate = fileinfo.LastWriteTime;
                        targetfiles.FileSize = fileinfo.Length;
                        targetfiles.DispRelativePath = fileinfo.Directory.ToString().Replace(@txtBasePath.Text + "\\", "").Replace(@txtBasePath.Text, "");

                        listPasteFile.Add(targetfiles);
                    }
                }

                if (myData.GetDataPresent(DataFormats.FileDrop))
                {
                    string[] arrfiles = (string[])myData.GetData(DataFormats.FileDrop);
                    foreach (string file in arrfiles)
                    {
                        FileInfo fileinfo = new FileInfo(file);

                        TargetFiles targetfiles = new TargetFiles();
                        targetfiles.FileInfo = fileinfo;
                        targetfiles.ListUpdateDate = fileinfo.LastWriteTime;
                        targetfiles.FileSize = fileinfo.Length;
                        targetfiles.DispRelativePath = fileinfo.Directory.ToString().Replace(@txtBasePath.Text + "\\", "").Replace(@txtBasePath.Text, "");

                        listPasteFile.Add(targetfiles);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return listPasteFile;
        }

        private bool CanGetDirectoryInfo()
        {
            if (Validation.GetHasError(txtBasePath))
                return false;

            if (Validation.GetHasError(txtLabelPath))
                return false;

            if (Validation.GetHasError(txtKoreanPornoPath))
                return false;

            return true;

        }

        private void OnDataGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 「wpf datagrid checkbox single click」で検索
            // 参考：http://social.msdn.microsoft.com/Forums/ja-JP/wpfja/thread/8a9a0654-1aff-4144-9167-232b2a91fafe/
            //       http://wpf.codeplex.com/wikipage?title=Single-Click Editing&ProjectName=wpf
            DataGridCell cell = sender as DataGridCell;

            bool IsClickDelete = false;
            if (cell.Column.Header.Equals("削除"))
                IsClickDelete = true;

            if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
            {
                TargetFiles selStartFile = (TargetFiles)dgridArrangementTarget.SelectedItem;

                if (selStartFile != null)
                {
                    DataGridRow row = FindVisualParent<DataGridRow>(cell);
                    TargetFiles selEndFile = row.Item as TargetFiles;
                    //Debug.Print("Shiftキーが押されたよ name [" + selStartFile.Name + "] ～ [" + selEndFile.Name + "]");

                    bool selStart = false;
                    foreach (TargetFiles file in dgridArrangementTarget.ItemsSource)
                    {
                        if (file.Name.Equals(selStartFile.Name))
                            selStart = true;

                        if (selStart)
                        {
                            if (IsClickDelete)
                            {
                                file.IsDeleted = true;
                                file.IsSelected = false;
                            }
                            else
                                file.IsSelected = true;
                        }

                        if (file.Name.Equals(selEndFile.Name))
                            break;
                    }

                    return;
                }
            }

            // 編集可能なセルの場合のみ実行
            if (cell != null && !cell.IsEditing && !cell.IsReadOnly)
            {
                // フォーカスが無い場合はフォーカスを取得
                if (!cell.IsFocused)
                    cell.Focus();

                DataGrid dataGrid = FindVisualParent<DataGrid>(cell);
                if (dataGrid != null)
                {
                    if (dataGrid.SelectionUnit != DataGridSelectionUnit.FullRow)
                    {
                        if (!cell.IsSelected)
                            cell.IsSelected = true;
                    }
                    else
                    {
                        DataGridRow row = FindVisualParent<DataGridRow>(cell);

                        TargetFiles selFile = row.Item as TargetFiles;
                        if (row != null && !row.IsSelected)
                        {
                            if (IsClickDelete)
                            {
                                if (selFile.IsDeleted)
                                    selFile.IsDeleted = false;
                                else
                                {
                                    selFile.IsDeleted = true;
                                    selFile.IsSelected = false;
                                }
                            }
                            else
                            {
                                if (selFile.IsSelected)
                                    selFile.IsSelected = false;
                                else
                                    selFile.IsSelected = true;
                            }
                        }
                        else
                        {
                            if (IsClickDelete)
                            {
                                if (row.IsSelected && selFile.IsDeleted)
                                    row.IsSelected = false;

                                selFile.IsDeleted = false;
                            }
                            else
                            {
                                if (row.IsSelected && selFile.IsSelected)
                                    row.IsSelected = false;

                                selFile.IsSelected = false;
                            }
                        }
                    }
                }
            }
        }
        static T FindVisualParent<T>(UIElement element) where T : UIElement
        {
            UIElement parent = element;
            while (parent != null)
            {
                T correctlyTyped = parent as T;
                if (correctlyTyped != null)
                {
                    return correctlyTyped;
                }

                parent = VisualTreeHelper.GetParent(parent) as UIElement;
            }
            return null;
        }

        private void btnExecuteDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("MOVIE_IMPORT_DATAから削除して、削除選択されたファイルをゴミ箱へ移していいですか？", "削除確認", MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.Cancel)
                return;

            FilesRegisterService service = new FilesRegisterService(new DbConnection());

            service.targetImportData = dispinfoSelectMovieImportData;
            service.DeleteExecute(ColViewDestFiles.listTargetFiles);

            ColViewMovieImport.Refresh();

            txtChangeFileName.Text = "";
            txtbTag.Text = "";
            txtSearch.Text = "";
        }

        /// <summary>
        /// FileGenerateのモードの場合の全削除ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExecuteDeleteEdit_Click(object sender, RoutedEventArgs e)
        {
            List<TargetFiles> files = GetSelectCheckExistFiles();

            string message = "MOVIE_IMPORT_DATAから削除します";
            if (files.Count > 0)
                message = "MOVIE_IMPORT_DATAから削除して、選択された" + files.Count + "個のファイルをゴミ箱へ移していいですか？";

            MessageBoxResult result = MessageBox.Show(message, "FileGen全削除確認", MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.Cancel)
                return;

            foreach (TargetFiles file in files)
            {
                if (file.IsSelected)
                {
                    Debug.Print("削除フラグ ゴミ箱移動 [" + file.FileInfo.FullName + "]");
                    FileSystem.DeleteFile(
                        file.FileInfo.FullName,
                        UIOption.OnlyErrorDialogs,
                        RecycleOption.SendToRecycleBin);
                }
            }

            // MOVIE_IMPORTから削除
            MovieImportService serviceImport = new MovieImportService();
            serviceImport.DbDelete(dispinfoSelectMovieImportData, new MySqlDbConnection());

            btnFileGenSearch_Click(null, null);

            ColViewMovieImport.Refresh();

            ClearUIElement();
            ColViewMovieImport.Refresh();
        }

        private List<TargetFiles> GetSelectCheckExistFiles()
        {
            List<TargetFiles> files = new List<TargetFiles>();

            foreach (TargetFiles file in dgridCheckExistFiles.Items)
            {
                if (file.IsSelected)
                    files.Add(file);
            }

            return files;
        }

        private void btnExecuteNameChange_Click(object sender, RoutedEventArgs e)
       {
            if (dispctrlMode == MODE_DATECOPY)
            {
                DateCopyService serviceDataCopy = new DateCopyService();

                string message = "";
                try
                {
                    serviceDataCopy.SetSourceFile(dgridArrangementTarget);
                    serviceDataCopy.SetDestFile(dgridDestFile);

                    message = serviceDataCopy.Execute();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }

                txtStatusBar.Text = message;

                return;
            }
            // 入力チェック
            string filename = txtChangeFileName.Text;

            if (filename.Length <= 0)
            {
                MessageBox.Show("ファイル名に入力がありません");
                return;
            }

            if (!txtChangeFileName.Text.Equals(dispinfoSelectMovieImportData.Filename))
            {
                MessageBoxResult result = MessageBox.Show("ファイル名が変更されていますが宜しいですか？", "変更確認", MessageBoxButton.OKCancel);

                if (result == MessageBoxResult.Cancel)
                    return;
            }
            dispinfoSelectMovieImportData.Filename = txtChangeFileName.Text;

            FilesRegisterService service = new FilesRegisterService(new DbConnection());
            FileControl fileControl = new FileControl();

            try
            {
                DateCopyService.CheckDataGridSelectItem(dgridArrangementTarget, "上のファイル", 1);

                service.BasePath = txtBasePath.Text;
                service.DestFilename = txtChangeFileName.Text;
                service.LabelPath = txtLabelPath.Text;

                service.SetSourceFile(dgridArrangementTarget);

                // 選択したファイルのみを対象に内部プロパティへ設定
                service.SetSelectedOnlyFiles(ColViewDestFiles.listTargetFiles);
            }
            catch (Exception ex)
            {
                Debug.Write(ex);
                MessageBox.Show(ex.Message, "初期設定エラー");
                return;
            }

            if (dispinfoSelectMovieImportData.FileId > 0)
            {
                try
                {
                    service.targetImportData = dispinfoSelectMovieImportData;
                    // HD動画への変更の場合
                    HdUpdateService hdUpdateService = new HdUpdateService(new DbConnection());

                    hdUpdateService.BasePath = txtBasePath.Text;
                    hdUpdateService.SetSelectedOnlyFiles(ColViewDestFiles.listTargetFiles);

                    MovieFileContents contents = ColViewMovieFileContents.MatchId(dispinfoSelectMovieImportData.FileId);

                    if (contents == null)
                    {
                        MessageBox.Show("対象のデータが存在しません " + dispinfoSelectMovieImportData.FileId);
                        return;
                    }
                    hdUpdateService.Execute(dispinfoSelectMovieImportData, contents);

                    txtStatusBar.Text = "";
                }
                catch (Exception ex)
                {
                    Debug.Write(ex);
                    MessageBox.Show(ex.Message, "HD動画変更エラー");
                    return;
                }
            }
            else
            {
                // 動画情報などの登録の場合
                try
                {
                    // JPEGの変換用の情報を生成する（日付コピー等はまだ実行されない）
                    service.SetJpegActionInfo();

                    // 動画の変換用の情報を生成する（日付コピー等はまだ実行されない）
                    service.SetMovieActionInfo();

                    // データベースへ登録用の情報を生成する
                    service.SetDbMovieFilesInfo(dispinfoSelectMovieImportData, null);

                    service.DbExport();

                    // 動画、画像ファイルの移動、日付コピー等の実行
                    service.Execute();
                }
                catch (Exception exp)
                {
                    Debug.Write(exp);
                    MessageBox.Show(exp.Message);
                    return;
                }
                finally
                {
                    // 選択中のファイル一覧はクリアする（次の対象動画になってしまうので）
                    foreach (TargetFiles file in dgridDestFile.ItemsSource)
                        file.IsSelected = false;

                    // フィルターをクリアしないと再取得した直後に動作して不要なチェックが付いてしまう
                    dgridDestFile.Items.Filter = null;
                }
            }

            try
            {
                service.DeleteFiles(ColViewDestFiles.listTargetFiles);
            }
            catch (Exception ex)
            {
                MessageBox.Show("削除失敗 " + ex.Message);
            }

            ColViewMovieImport.Refresh();

            txtChangeFileName.Text = "";
            txtbTag.Text = "";
            txtSearch.Text = "";
        }

        private void menuitemNameCopy(object sender, RoutedEventArgs e)
        {
            //コピーするファイルのパスをStringCollectionに追加する
            TargetFiles file = (TargetFiles)dgridDestFile.SelectedItem;
            string NameWithoutExt = file.FileInfo.Name.Replace(file.FileInfo.Extension, "");
            Clipboard.SetText(NameWithoutExt);
        }

        private void OnGridTargetDisplay(object sender, RoutedEventArgs e)
        {
            string SelectMenuHeader = "";
            MenuItem menu = (MenuItem)sender;

            if (menu != null)
            {
                SelectMenuHeader = menu.Header.ToString();

                if (SelectMenuHeader.IndexOf("RARファイル一致") >= 0)
                    dispctrlMode = MODE_NORMALRAR;
                else
                    dispctrlMode = MODE_NORMALMOVIE;
            }

            string searchStr = (dispinfoSelectMovieImportData != null) ? dispinfoSelectMovieImportData.GetFileSearchString() : "";
            if (dispctrlMode == MODE_NORMALRAR)
            {
                menuitemRarFiles.IsChecked = true;
                menuitemMovieFiles.IsChecked = false;

                if (ColViewArrangementTarget != null)
                {
                    ColViewArrangementTarget.FilterSearchProductNumber = searchStr;
                    ColViewArrangementTarget.RefreshRarFile();
                }
            }
            else
            {
                menuitemRarFiles.IsChecked = false;
                menuitemMovieFiles.IsChecked = true;

                if (!CanGetDirectoryInfo())
                    return;

                if (ColViewArrangementTarget != null)
                {
                    ColViewArrangementTarget.FilterSearchProductNumber = searchStr;
                    ColViewArrangementTarget.Refresh(collection.FileGeneTargetFilesCollection.REGEX_MOVIE_EXTENTION);
                }
            }
            if (ColViewArrangementTarget != null)
            {
                ColViewDestFiles.FilterSearchProductNumber = searchStr;
                ColViewDestFiles.Refresh(collection.FileGeneTargetFilesCollection.REGEX_MOVIE_EXTENTION);
            }
        }

        private void dgridArrangementTarget_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TargetFiles file = (TargetFiles)dgridArrangementTarget.CurrentItem;

            int i = 0;
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            OnGridTargetDisplay(null, null);

            if (!CanGetDirectoryInfo())
                return;

            ColViewDestFiles.Refresh(collection.FileGeneTargetFilesCollection.REGEX_MOVIE_EXTENTION);

            SetDataGridDefaultSelectSetting();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            ColViewArrangementTarget.FilterSearchProductNumber = txtSearch.Text;
            ColViewArrangementTarget.Refresh();
            ColViewDestFiles.FilterSearchProductNumber = txtSearch.Text;
            ColViewDestFiles.Refresh();

            btnExecuteNameChange.Focus();
        }

        private void btnSearchCancel_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = "";
            txtStatusBar.Text = "";
            dgridArrangementTarget.Items.Filter = null;
            dgridDestFile.Items.Filter = null;
        }

        private void menuitemRenameFile_Click(object sender, RoutedEventArgs e)
        {
            gridRenameFile.Visibility = System.Windows.Visibility.Visible;

            TargetFiles selStartFile = (TargetFiles)dgridArrangementTarget.SelectedItem;
            txtRenameSourceFile.Text = selStartFile.Name;

        }

        private void btnRenameFileCancel_Click(object sender, RoutedEventArgs e)
        {
            gridRenameFile.Visibility = System.Windows.Visibility.Hidden;
        }

        private void btnRenameFileExecute_Click(object sender, RoutedEventArgs e)
        {
            string SourcePathFile = System.IO.Path.Combine(txtBasePath.Text, txtRenameSourceFile.Text);
            string DestPathFile = System.IO.Path.Combine(txtBasePath.Text, txtRenameDestFilename.Text);
            File.Move(SourcePathFile, DestPathFile);

            gridRenameFile.Visibility = System.Windows.Visibility.Hidden;
        }

        private void menuitemDeleteFile_Click(object sender, RoutedEventArgs e)
        {
            TargetFiles file = (TargetFiles)dgridArrangementTarget.SelectedItem;
            FileSystem.DeleteFile(
                file.FileInfo.FullName,
                UIOption.OnlyErrorDialogs,
                RecycleOption.SendToRecycleBin);
        }

        private void dgridDestFile_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGrid dgrid = sender as DataGrid;
            TargetFiles file = (TargetFiles)dgrid.SelectedItem;

            if (file != null)
                Process.Start(file.FileInfo.FullName);
        }

        private void Window_SizeChanged_1(object sender, SizeChangedEventArgs e)
        {
            dgridSelectTargetFilename.Width = this.ActualWidth - 10;
            txtStatusBar.Width = stsbaritemDispDetail.ActualWidth;
        }

        private void txtChangeFileName_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            dgridSelectTargetFilename.ItemsSource = ColViewMovieImport.collection;

            if (dispctrlMode == MODE_FILENAMEGENERATE)
                lgridFilenameGenerate.Visibility = System.Windows.Visibility.Collapsed;
            else
                lgridMain.Visibility = System.Windows.Visibility.Collapsed;

            gridSelectTargetFilename.Visibility = System.Windows.Visibility.Visible;
        }

        private void btnSelectCancel_Click(object sender, RoutedEventArgs e)
        {
            if (dispctrlMode == MODE_FILENAMEGENERATE)
                lgridFilenameGenerate.Visibility = System.Windows.Visibility.Visible;
            else
                lgridMain.Visibility = System.Windows.Visibility.Visible;

            dispinfoSelectMovieImportData = null;

            gridSelectTargetFilename.Visibility = System.Windows.Visibility.Hidden;
        }

        private void dgridSelectTargetFilename_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            dispinfoSelectMovieImportData = (MovieImportData)dgridSelectTargetFilename.SelectedItem;

            if (dispctrlMode == MODE_FILENAMEGENERATE)
            {
                string searchText = dispinfoSelectMovieImportData.GetFilterProductNumber() + " " + dispinfoSelectMovieImportData.DownloadFiles;

                txtFileGeneSearchText.Text = searchText.Trim();

                ColViewFileGeneTargetFiles.FilterSearchProductNumber = searchText.Trim();
                ColViewFileGeneTargetFiles.Refresh();

                if (!string.IsNullOrEmpty(dispinfoSelectMovieImportData.SearchResult.Trim()))
                {
                    string[] arrSearchResult = dispinfoSelectMovieImportData.SearchResult.Split('\n');

                    List<SearchResultData> searchResultList = new List<SearchResultData>();
                    foreach (string oneData in arrSearchResult)
                    {
                        Regex regex = new Regex("http[s]*://.*");
                        if (regex.IsMatch(oneData))
                        {
                            Match m = regex.Match(oneData);
                            string url = m.Value;
                            searchResultList.Add(new SearchResultData(oneData.Replace(url, "").Trim(), url));
                        }
                    }
                    txtStatusBar.Text = arrSearchResult[0] + " " + dispinfoSelectMovieImportData.Detail;
                    cmbSearchResult.ItemsSource = searchResultList;

                    if (searchResultList.Count > 0)
                        cmbSearchResult.SelectedIndex = 0;

                    txtbSearchResultCount.Text = Convert.ToString(searchResultList.Count);
                }
                else
                {
                    cmbSearchResult.ItemsSource = null;
                    txtbSearchResultCount.Text = Convert.ToString(0);
                    txtStatusBar.Text = dispinfoSelectMovieImportData.Detail;
                }

                SetUIElementFromImportData(dispinfoSelectMovieImportData);
                btnPasteActressesSearch_Click(null, null);

                foreach (TargetFiles file in ColViewFileGeneTargetFiles.ColViewListTargetFiles)
                {
                    if (Regex.IsMatch(file.Name, FileGeneTargetFilesCollection.REGEX_RARONLY_EXTENTION,
                        RegexOptions.IgnoreCase))
                        file.IsDeleted = true;
                    else
                        file.IsSelected = true;
                }

                lgridFilenameGenerate.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                txtSearch.Text = dispinfoSelectMovieImportData.GetFileSearchString();

                txtbSourceFilename.Text = dispinfoSelectMovieImportData.Filename;
                txtChangeFileName.Text = dispinfoSelectMovieImportData.Filename;

                List<MovieFileContents> matchFileContentsList = ColViewMovieFileContents.MatchProductNumber(dispinfoSelectMovieImportData.ProductNumber);

                if ((bool)dispinfoSelectMovieImportData.RarFlag)
                {
                    ChangeModeNormalRarExecute(null, null);

                    foreach (TargetFiles file in dgridDestFile.ItemsSource)
                        file.IsSelected = false;
                }
                else
                {
                    string searchStr = (dispinfoSelectMovieImportData != null) ? dispinfoSelectMovieImportData.GetFileSearchString() : "";
                    collection.FileGeneTargetFilesCollection ColViewCheckRarFiles = new collection.FileGeneTargetFilesCollection(txtBasePath.Text, collection.FileGeneTargetFilesCollection.REGEX_RARONLY_EXTENTION, searchStr);
                    ColViewCheckRarFiles.Execute();

                    foreach (var data in ColViewCheckRarFiles.ColViewListTargetFiles)
                    {
                        MessageBoxResult result = MessageBox.Show("RARファイルが存在します、編集に移動しますか？", "確認", MessageBoxButton.OKCancel);

                        if (result == MessageBoxResult.OK)
                        {
                            txtFileGeneSearchText.Text = dispinfoSelectMovieImportData.GetFilterProductNumber();

                            ColViewFileGeneTargetFiles.FilterSearchProductNumber = txtFileGeneSearchText.Text;
                            ColViewFileGeneTargetFiles.Refresh();

                            SetUIElementFromImportData(dispinfoSelectMovieImportData);

                            lgridFilenameGenerate.Visibility = System.Windows.Visibility.Visible;

                            CahngeModeFilenameGenerateExecute(null, null);

                            gridSelectTargetFilename.Visibility = Visibility.Hidden;

                            return;
                        }
                    }

                    ChangeModeNormalMovieExecute(null, null);
                }

                if (matchFileContentsList.Count > 0)
                {
                    MovieFileContents matchFileContents = matchFileContentsList[0];

                    MovieImportData impData = new MovieImportData(matchFileContents.Name);
                    txtbExistFileId.Text = Convert.ToString(matchFileContents.Id);
                    txtbExistTitle.Text = impData.Title;
                }
                else
                {
                    txtbExistFileId.Text = "";
                    txtbExistTitle.Text = "";
                }

                if (dispinfoSelectMovieImportData.FileId > 0)
                    txtbTag.Background = new SolidColorBrush(Colors.PaleGreen);
                else
                    txtbTag.Background = null;

                txtbTag.Text = dispinfoSelectMovieImportData.Tag;
                txtbRating.Text = Convert.ToString(dispinfoSelectMovieImportData.Rating);

                lgridMain.Visibility = System.Windows.Visibility.Visible;

                SetDataGridDefaultSelectSetting();

                btnExecuteNameChange.Focus();
            }

            txtbTargetIdInfo.Text = Convert.ToString(dispinfoSelectMovieImportData.Id) + "/" + Convert.ToString(dispinfoSelectMovieImportData.FileId);

            gridSelectTargetFilename.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// dgridArramentTargetは動画、dgridDestFileは検索一致のファイルを全て選択チェックの状態にする
        /// </summary>
        private void SetDataGridDefaultSelectSetting()
        {
            dgridArrangementTarget.SelectedItem = ColViewArrangementTarget.GetSelectTargetMovieFile();

            List<TargetFiles> filesList = ColViewArrangementTarget.GetSelectTargetFiles();
            foreach (TargetFiles file in filesList)
            {
                foreach (TargetFiles item in dgridDestFile.ItemsSource)
                {
                    if (file.Name.Equals(item.Name))
                        item.IsSelected = true;
                }
            }
        }

        private void btnBasePathPaste_Click(object sender, RoutedEventArgs e)
        {
            txtBasePath.Text = ClipBoardCommon.GetTextPath();
        }

        private void btnLabelPathPaste_Click(object sender, RoutedEventArgs e)
        {
            txtLabelPath.Text = ClipBoardCommon.GetTextPath();
        }

        private void Window_Closed_1(object sender, EventArgs e)
        {
            settingControl.Save(txtBasePath.Text, txtLabelPath.Text, txtKoreanPornoPath.Text, txtKoreanPornoExportPath.Text);
        }

        /// <summary>
        /// ImportDataを取得
        /// btnPasteTitleText_Click, btnPasteTextRefresh_Click で使用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private MovieImportData GetImportData(string myPasteText)
        {
            MovieImportData importData = null;
            if (myPasteText.IndexOf("[AV") == 0
                || myPasteText.IndexOf("[IV") == 0
                || myPasteText.IndexOf("[裏") == 0)
            {
                importData = new MovieImportData(myPasteText);

                txtbFileGenFileId.Text = Convert.ToString(importData.FileId);
                txtKind.Text = Convert.ToString(importData.Kind);
                txtFilenameGenDate.Text = importData.ProductDate.ToString("yyyy/MM/dd");
                txtProductNumber.Text = importData.ProductNumber;
                txtMaker.Text = importData.StrMaker;
                txtTitle.Text = importData.Title;
                txtPackage.Text = importData.Package;
                txtThumbnail.Text = importData.Thumbnail;
                tbtnFileGenHdUpdate.IsChecked = importData.HdFlag;

                if (importData.RarFlag == true)
                    tbtnFileGeneTextAddRar.IsChecked = true;

                dispinfoSelectMovieImportData = importData;
                ColViewFileGeneTargetFiles.FilterSearchProductNumber = dispinfoSelectMovieImportData.GetFilterProductNumber();
                ColViewFileGeneTargetFiles.Refresh();
            }
            else
            {
                importData = new MovieImportData();
                // メーカー情報と合わせるための製品番号、HDかどうかの情報をParse
                importData.ParseFromPasteText(myPasteText);
            }

            return importData;
        }

        /// <summary>
        /// 入力されている貼り付けテキストでメーカー情報再取得、反映
        /// btnPasteTitleText_Click, btnPasteTextRefresh_Click で使用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefrectionMatchMaker(List<MovieMaker> myMatchMakerList)
        {
            try
            {
                if (myMatchMakerList == null || myMatchMakerList.Count() <= 0)
                {
                    txtStatusBar.Text = "一致するメーカーが存在しませんでした";
                    dispinfoSelectMovieImportData.SetPickupTitle();
                    SetUIElementFromImportData(dispinfoSelectMovieImportData);
                }
                else if (myMatchMakerList.Count() == 1)
                {
                    dispinfoSelectMovieImportData.SetMaker(myMatchMakerList[0]);
                    dispinfoSelectMovieImportData.SetPickupTitle();
                    SetUIElementFromImportData(dispinfoSelectMovieImportData);
                }
                else
                {
                    dispinfoSelectMovieImportData.SetMaker(myMatchMakerList[0]);
                    dispinfoSelectMovieImportData.SetPickupTitle();
                    SetUIElementFromImportData(dispinfoSelectMovieImportData);

                    dgridMakers.ItemsSource = null;
                    dgridMakers.ItemsSource = myMatchMakerList;

                    lgridMakers.Visibility = System.Windows.Visibility.Visible;

                    // Autoの設定にする
                    ScreenDisableBorder.Width = Double.NaN;
                    ScreenDisableBorder.Height = Double.NaN;

                    isSelectSameMaker = true;

                    return;
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex);
                MessageBox.Show(ex.Message);
                return;
            }

            ColViewFileGeneTargetFiles.FilterSearchProductNumber = dispinfoSelectMovieImportData.GetFilterProductNumber();
            txtFileGeneSearchText.Text = ColViewFileGeneTargetFiles.FilterSearchProductNumber;

            ColViewFileGeneTargetFiles.Refresh();
        }

        private void btnPasteTextRefresh_Click(object sender, RoutedEventArgs e)
        {

            // 一致したメーカーで、再反映
            //RefrectionMatchMaker(ColViewMaker.GetMatchData(importData));

            WebRequest request = WebRequest.Create("http://192.168.11.8:5000/import/" + txtbFileGenImportId.Text);

            string json = "";
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                json = reader.ReadToEnd();
                Debug.Print(json);
            }

            //var jsonObj = DynamicJson.Parse("{\"ABC\": \"DEF\"}");
            var jsonObj = DynamicJson.Parse(json);

            string p_num = (string)jsonObj.import.p_number;

            if (p_num.Length > 0 && txtProductNumber.Text != p_num)
            {
                MessageBox.Show("品番が変わりました [" + p_num + "]");
                txtProductNumber.Text = p_num;
                txtFilenameGenDate.Text = jsonObj.import.sell_date;

                if (p_num.IndexOf("_") >= 0)
                    txtMaker.Text = "HEY動画：" + jsonObj.import.seller;
                else
                    txtMaker.Text = jsonObj.import.seller;
            }
            int i = 1;
            //Debug.Print(jsonObj.ABC);
            //WriteLine(jsonString); // {"Name":"Kato Jun","Age":31}
            //MovieImportData importData = GetImportData(txtTitleText.Text);

        }

        private void btnPasteTitleText_Click(object sender, RoutedEventArgs e)
        {
            txtStatusBar.Text = "";

            string titletext = ClipBoardCommon.GetText();

            if (titletext.Trim().Length <= 10)
            {
                bool isDate = false;
                try
                {
                    DateTime dt = Convert.ToDateTime(titletext);
                    txtFilenameGenDate.Text = dt.ToString("yyyy/MM/dd");
                    isDate = true;
                }
                catch(Exception)
                {

                }
                if (isDate)
                    return;
            }

            dispinfoSelectMovieImportData = null;
            ClearUIElement();

            MovieImportData importData = GetImportData(titletext);

            List<MovieMaker> listMatchMaker = ColViewMaker.GetMatchData(importData);

            if (importData.ProductNumber == null || importData.ProductNumber.Length <= 0)
            {
                if (listMatchMaker.Count == 1)
                {
                    importData.SetMaker(listMatchMaker[0]);
                    importData.SetProductNumber();
                }
            }

            if (String.IsNullOrEmpty(importData.ProductNumber))
            {
                // MOVIE_IMPORT_DATAに既存にデータがが存在すれば表示
                dispinfoSelectMovieImportData = ColViewMovieImport.GetDataByProductId(importData.ProductNumber);

                List<MovieFileContents> matchList = null;

                // HDの場合は、MOVIE_FILESからも一致するデータが存在するかを取得
                matchList = ColViewMovieFileContents.MatchProductNumber(importData.ProductNumber);

                if (matchList.Count == 1)
                {
                    MovieFileContents fileContents = matchList[0];
                    dispinfoSelectMovieImportData = new MovieImportData(fileContents.Name);

                    dispinfoSelectMovieImportData.CopyText = titletext;
                    dispinfoSelectMovieImportData.FileId = fileContents.Id;
                    dispinfoSelectMovieImportData.HdKind = importData.HdKind;
                    dispinfoSelectMovieImportData.HdFlag = true;
                    dispinfoSelectMovieImportData.Tag = fileContents.Tag;
                    dispinfoSelectMovieImportData.SetPickupTitle(fileContents);
                }
                else if (matchList.Count > 1)
                {
                    string msg = "対象のMOVIE_FILE_CONTENTSが複数件存在します";

                    foreach (MovieFileContents data in matchList)
                    {
                        msg += "\n" + data.Name;
                    }
                    txtStatusBar.Text = msg;
                }
                else
                {
                    txtStatusBar.Text = "対象のMOVIE_FILE_CONTENTSは存在しません";
                }

                txtTitleText.Text = titletext;

                if (dispinfoSelectMovieImportData != null)
                {
                    SetUIElementFromImportData(dispinfoSelectMovieImportData);

                    ColViewFileGeneTargetFiles.FilterSearchProductNumber = dispinfoSelectMovieImportData.GetFilterProductNumber();
                    txtFileGeneSearchText.Text = ColViewFileGeneTargetFiles.FilterSearchProductNumber;
                    ColViewFileGeneTargetFiles.Refresh();

                    return;
                }
            }

            if (dispinfoSelectMovieImportData == null)
                dispinfoSelectMovieImportData = importData;

            RefrectionMatchMaker(listMatchMaker);
        }

        private void dgridMakers_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // 文字列一致のメーカー複数の選択の場合
            if (isSelectSameMaker)
            {
                dispinfoSelectMovieImportData.SetMaker((MovieMaker)dgridMakers.SelectedItem);
                dispinfoSelectMovieImportData.SetPickupTitle();
                SetUIElementFromImportData(dispinfoSelectMovieImportData);

                isSelectSameMaker = false;

                ColViewFileGeneTargetFiles.FilterSearchProductNumber = (dispinfoSelectMovieImportData != null) ? dispinfoSelectMovieImportData.GetFilterProductNumber() : txtProductNumber.Text;
                ColViewFileGeneTargetFiles.Refresh();

                ButtonMakerClose(null, null);
            }
            isSelectSameMaker = false;
        }

        private void btnPasteDate_Click(object sender, RoutedEventArgs e)
        {
            string ClipboardText = ClipBoardCommon.GetText();

            txtFilenameGenDate.Text = ClipboardText;
            GenerateFilename(null, null);
        }

        private void txtMaker_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            lgridMakers.Visibility = System.Windows.Visibility.Visible;

            // Autoの設定にする
            ScreenDisableBorder.Width = Double.NaN;
            ScreenDisableBorder.Height = Double.NaN;

            dgridMakers.ItemsSource = ColViewMaker.ColViewListMakers;
        }

        private void ButtonMakerClose(object sender, RoutedEventArgs e)
        {
            isSelectSameMaker = false;

            if (lgridRegistMaker.Visibility == System.Windows.Visibility.Visible)
            {
                lgridRegistMaker.Visibility = System.Windows.Visibility.Collapsed;
                return;
            }
            if (lgridMakers.Visibility == System.Windows.Visibility.Visible)
            {
                lgridMakers.Visibility = System.Windows.Visibility.Collapsed;
                return;
            }
        }

        private void btnRegistMaker_Click(object sender, RoutedEventArgs e)
        {
            MovieMaker regdata = new MovieMaker();

            regdata.Name = txtRegistMakerName.Text;
            regdata.Label = txtRegistMakerLabel.Text;
            regdata.Kind = Convert.ToInt32(txtRegistMakerKind.Text);
            regdata.MatchStr = txtRegistMakerMatchStr.Text;
            regdata.MatchProductNumber = txtRegistMatchProductNumber.Text;

            if (txtRegistId.Text.Length > 0)
            {
                regdata.Id = Convert.ToInt32(txtRegistId.Text);
                regdata.DbUpdate(null);
            }
            else
                regdata.DbExport(null);

            lgridRegistMaker.Visibility = System.Windows.Visibility.Collapsed;

            dgridMakers.ItemsSource = null;
            ColViewMaker.Refresh();

            txtRegistId.Text = "";
            txtRegistMakerName.Text = "";
            txtRegistMakerLabel.Text = "";
            txtRegistMakerKind.Text = "";
            txtRegistMakerMatchStr.Text = "";
            txtRegistMatchProductNumber.Text = "";

            dgridMakers.ItemsSource = ColViewMaker.ColViewListMakers;

            return;
        }

        /// <summary>
        /// 追加・編集ボタン押下時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpenRegistMaker_Click(object sender, RoutedEventArgs e)
        {
            if (lgridMakers.Visibility == System.Windows.Visibility.Visible)
            {
                var sel = dgridMakers.SelectedItems;

                if (sel.Count == 1)
                {
                    MovieMaker data = sel[0] as MovieMaker;
                    txtRegistId.Text = data.Id.ToString();
                    txtRegistMakerName.Text = data.Name;
                    txtRegistMakerKind.Text = data.Kind.ToString();
                    txtRegistMakerLabel.Text = data.Label;
                    txtRegistMakerMatchStr.Text = data.MatchStr;
                    txtRegistMatchProductNumber.Text = data.MatchProductNumber;
                }
            }
            else
            {
                txtRegistId.Text = "";
                if (txtMaker.Text.IndexOf("：") > 0)
                {
                    string[] splitStr = { "：" };
                    string[] makerandlabel = txtMaker.Text.Split(splitStr, StringSplitOptions.RemoveEmptyEntries);
                    txtRegistMakerName.Text = makerandlabel[0];
                    txtRegistMakerLabel.Text = makerandlabel[1];
                }
                else
                {
                    //txtRegistMakerLabel = "";
                    txtRegistMakerName.Text = txtMaker.Text;
                }
                txtRegistMakerKind.Text = txtKind.Text;
            }
            lgridRegistMaker.Visibility = System.Windows.Visibility.Visible;
        }

        private MovieImportData GetImportDataFromUIElement()
        {
            MovieImportData movieImportData = new MovieImportData();

            if (txtbFileGenImportId.Text.Length > 0)
                movieImportData.Id = Convert.ToInt32(txtbFileGenImportId.Text);

            movieImportData.CopyText = txtTitleText.Text;

            if (txtbFileGenFileId.Text.Length > 0)
                movieImportData.FileId = Convert.ToInt32(txtbFileGenFileId.Text);
            if (txtKind.Text.Length > 0)
                movieImportData.Kind = Convert.ToInt32(txtKind.Text);
            movieImportData.ProductNumber = txtProductNumber.Text;
            if (Validation.GetHasError(txtFilenameGenDate))
                movieImportData.ProductDate = new DateTime(1900, 1, 1);
            else
                movieImportData.StrProductDate = txtFilenameGenDate.Text;
            movieImportData.StrMaker = txtMaker.Text;
            movieImportData.Title = txtTitle.Text;
            movieImportData.HdFlag = tbtnFileGenHdUpdate.IsChecked;
            movieImportData.RarFlag = tbtnFileGeneTextAddRar.IsChecked;
            movieImportData.SplitFlag = tbtnFileGenSplit.IsChecked;
            movieImportData.NameOnlyFlag = tbtnFileGenModeFilenameOnly.IsChecked;
            movieImportData.Tag = txtTag.Text;
            movieImportData.Package = txtPackage.Text;
            movieImportData.Thumbnail = txtThumbnail.Text;

            movieImportData.GenerateFilename();
            movieImportData.Filename = txtFilenameGenerate.Text;
            if (txtFileGenRating.Text.Length > 0)
                movieImportData.Rating = Convert.ToInt32(txtFileGenRating.Text);

            movieImportData.GenerateFilename();

            return movieImportData;
        }

        public BitmapImage GetImageStream(string myImagePathname)
        {
            if (!System.IO.File.Exists(myImagePathname))
                return null;

            BitmapImage bitmap = new BitmapImage();
            var stream = System.IO.File.OpenRead(myImagePathname);
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.StreamSource = stream;
            bitmap.EndInit();
            stream.Close();
            stream.Dispose();
            int width = bitmap.PixelWidth;
            int height = bitmap.PixelHeight;
            return bitmap;
        }

        private void SetUIElementFromImportData(MovieImportData myData)
        {
            if (myData.FileId > 0)
                txtbFileGenFileId.Text = Convert.ToString(myData.FileId);

            if (myData.Id > 0)
                txtbFileGenImportId.Text = Convert.ToString(myData.Id);

            txtbPostDate.Text = myData.JavPostDate.ToString("yyyy/MM/dd dddd HH:mm:ss");
            txtTitleText.Text = myData.CopyText;
            txtKind.Text = myData.Kind.ToString();
            if (myData.ProductDate.Year > 1900)
                txtFilenameGenDate.Text = myData.ProductDate.ToString("yyyy/MM/dd");
            else
                txtFilenameGenDate.Text = "";
            txtProductNumber.Text = myData.ProductNumber;
            txtMaker.Text = myData.StrMaker;
            txtTitle.Text = myData.Title;
            txtTag.Text = myData.Tag;
            txtPackage.Text = myData.Package;
            txtThumbnail.Text = myData.Thumbnail;

            tbtnFileGeneTextAddRar.IsChecked = myData.RarFlag;
            tbtnFileGenSplit.IsChecked = myData.SplitFlag;
            tbtnFileGenModeFilenameOnly.IsChecked = myData.NameOnlyFlag;
            tbtnFileGenModeFilenameOnly_Click(null, null);
            txtFilenameGenerate.Text = myData.Filename;
            txtFileGenRating.Text = Convert.ToString(myData.Rating);

            if (myData.FileId > 0)
                tbtnFileGenHdUpdate.IsChecked = true;

            string packagePathname = Path.Combine(JpegStorePath, myData.Package);
            string thumbnailPathname = Path.Combine(JpegStorePath, myData.Thumbnail);
            if (File.Exists(packagePathname))
            {
                imagePackage.Width = lgridDataGridFiles.ColumnDefinitions[0].ActualWidth;
                imagePackage.Height = 300;
                imagePackage.Source = this.GetImageStream(packagePathname);
            }

            if (File.Exists(thumbnailPathname))
            {
                imagePackage.Width = lgridDataGridFiles.ColumnDefinitions[1].ActualWidth;
                imagePackage.Height = 300;
                imageThumbnail.Source = this.GetImageStream(thumbnailPathname);
            }
        }

        private void ClearUIElement()
        {
            txtbFileGenImportId.Text = "";
            txtbFileGenFileId.Text = "";
            txtTitleText.Text = "";
            if (chkKindFixed.IsChecked == null || !(bool)chkKindFixed.IsChecked) txtKind.Text = "";
            txtProductNumber.Text = "";
            txtMaker.Text = "";
            txtTitle.Text = "";
            txtTag.Text = "";
            txtPackage.Text = "";
            txtThumbnail.Text = "";
            tbtnFileGeneTextAddRar.IsChecked = false;
            tbtnFileGenSplit.IsChecked = false;
            txtFilenameGenerate.Text = "";
            tbtnFileGenHdUpdate.IsChecked = false;

        }

        private void GenerateFilename(object sender, RoutedEventArgs e)
        {
            if (Validation.GetHasError(txtFilenameGenDate))
                return;

            MovieImportData movieImportData = GetImportDataFromUIElement();

            txtFilenameGenerate.Text = movieImportData.Filename;

            return;

        }

        private void btnOpenRegistMaker_Click(object sender, MouseButtonEventArgs e)
        {
            Debug.Print("TEST btnOpenRegistMaker_Click");
        }

        private void btnFileGenUpdate_Click(object sender, RoutedEventArgs e)
        {
            MovieImportService service = new service.MovieImportService();

            if (!GetChecked(tbtnFileGenModeFilenameOnly) && Validation.GetHasError(txtFilenameGenDate))
            {
                txtStatusBar.Text = "日付が正しく入力されていないため登録できません";
                return;
            }

            string importId = txtbFileGenImportId.Text;

            if (importId.Length > 0)
            {
                MovieImportData movieImportData = GetImportDataFromUIElement();
                movieImportData.Id = Convert.ToInt32(importId);

                service.DbUpdate(movieImportData, new MySqlDbConnection());

                ClearUIElement();
            }
            else
            {
                MovieImportData movieImportData = GetImportDataFromUIElement();

                movieImportData = service.DbExport(movieImportData, new MySqlDbConnection());

                ClearUIElement();
            }
            var selItem = dgridSelectTargetFilename.SelectedItem;

            int selIndex = dgridSelectTargetFilename.SelectedIndex;

            ColViewMovieImport.Refresh();

            dgridSelectTargetFilename.SelectedItem = selItem;
        }

        private void btnPasteActresses_Click(object sender, RoutedEventArgs e)
        {
            string ClipboardText = ClipBoardCommon.GetText();

            txtTag.Text = dispinfoSelectMovieImportData.ConvertActress(ClipboardText, ",");

            string message = "";
            foreach(ReplaceInfoData replaceInfoData in dispctrlListReplaceInfoActress)
            {
                if (txtTag.Text.IndexOf(replaceInfoData.Source) >= 0)
                {
                    txtTag.Text = txtTag.Text.Replace(replaceInfoData.Source, replaceInfoData.Destination);
                    message = message + "、" + replaceInfoData.Source + "->" + replaceInfoData.Destination;
                }
            }
            txtStatusBar.Text = message;

            GenerateFilename(null, null);
        }

        private void dgridMakers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var colSel = dgridMakers.SelectedItems;

            if (dispinfoSelectDataGridMakers == null)
                dispinfoSelectDataGridMakers = new List<MovieMaker>();
            else
                dispinfoSelectDataGridMakers.Clear();

            foreach (MovieMaker data in colSel)
                dispinfoSelectDataGridMakers.Add(data);
        }

        private void dgridMakers_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
                OnSelectionRowDelete(null, null);
        }

        private void OnSelectionRowDelete(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("選択行を削除して宜しいですか？", "削除確認", MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.Cancel)
                return;

            DbConnection dbcon = new DbConnection();

            try
            {
                dbcon.BeginTransaction("DELETE_ARTIST");
                foreach (MovieMaker data in dispinfoSelectDataGridMakers)
                {
                    data.DbDelete(dbcon);
                    Debug.Print("ID [" + data.Id + "]  Name [" + data.Name + "]");
                }
                dbcon.CommitTransaction();
            }
            catch (Exception ex)
            {
                Debug.Write(ex);
                MessageBox.Show(ex.Message, "エラー発生");
            }
        }

        private void dgridKoreanPorno_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dispinfoSelectDataGridKoreanPorno != null)
            {
                if (txtKoreanPornoExportPath.Text.Trim().Length <= 0)
                {
                    MessageBox.Show("出力先のパスが設定されていません");
                    return;
                }

                MovieImportData selData = dispinfoSelectDataGridKoreanPorno;
                TxtKoreanPornoName.Text = selData.CopyText;
                TxtKoreanPornoArchiveFile.Text = selData.ProductNumber;
                TxtKoreanPornoPostDate.Text = selData.JavPostDate.ToString("yyyy/MM/dd HH:mm:ss");
                TxtKoreanPornoTag.Text = selData.Tag;
                TxtKoreanPornoPostedIn.Text = selData.Actresses;

                KoreanPornoService service = new KoreanPornoService(txtKoreanPornoPath.Text, txtKoreanPornoExportPath.Text, (bool)ChkKoreanPornoMoveFolder.IsChecked);
                List<KoreanPornoFileInfo> listFiles = service.GetFileInfo(selData.CopyText, selData.JavPostDate, dispinfoSelectDataGridKoreanPorno.ProductNumber);

                if (listFiles == null)
                {
                    MessageBox.Show("まだ解凍されていません");
                    return;
                }
                else
                    dgridKoreanPornoFolder.ItemsSource = listFiles;
            }

            dgridKoreanPorno.Visibility = System.Windows.Visibility.Hidden;
            btnKoreanPornoExecute.Focus();
        }

        private void dgridKoreanPorno_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgridKoreanPorno.SelectedItems != null)
            {
                if (dgridKoreanPorno.SelectedItems.Count == 1)
                    dispinfoSelectDataGridKoreanPorno = (MovieImportData)dgridKoreanPorno.SelectedItem;
                else if (dgridKoreanPorno.SelectedItems.Count > 1)
                {
                    dispinfoSelectDataGridKoreanPorno = (MovieImportData)dgridKoreanPorno.SelectedItems[0];
                    txtStatusBar.Text = "複数選択しているので、先頭の選択行を対象とします";
                }
            }
        }

        private void btnKoreanPornoExecute_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<KoreanPornoFileInfo> fileList = (List<KoreanPornoFileInfo>)dgridKoreanPornoFolder.ItemsSource;

                Regex regexImage = new Regex(FileGeneTargetFilesCollection.REGEX_IMAGE_EXTENTION, RegexOptions.IgnoreCase);

                int movieCount = 0;
                foreach (KoreanPornoFileInfo data in fileList)
                {
                    if (data.IsSelected && regexImage.IsMatch(data.FileInfo.Name))
                        continue;
                    else if (data.IsSelected)
                        movieCount++;
                }

                bool ischeck = (bool)(ChkKoreanPornoMoveFolder.IsChecked == null ? false : ChkKoreanPornoMoveFolder.IsChecked);
                if (movieCount > 5 && ischeck == false)
                {
                    MessageBoxResult result = MessageBox.Show("動画ファイルの個数が5個を超えていて\nフォルダ構成のチェック無し、良い？", "確認", MessageBoxButton.OKCancel);

                    if (result == MessageBoxResult.Cancel)
                        return;

                }

                MovieImportData selData = dispinfoSelectDataGridKoreanPorno;
                selData.CopyText = TxtKoreanPornoName.Text;
                selData.ProductNumber = TxtKoreanPornoArchiveFile.Text;
                selData.Tag = TxtKoreanPornoTag.Text;
                StoreData storeData = ColViewStore.GetMatchByPath(txtKoreanPornoExportPath.Text);

                KoreanPornoService service = new KoreanPornoService(txtKoreanPornoPath.Text, txtKoreanPornoExportPath.Text, ChkKoreanPornoMoveFolder.IsChecked);
                service.ExecuteArrangement(selData, storeData, (List<KoreanPornoFileInfo>)dgridKoreanPornoFolder.ItemsSource);

                ColViewKoreanPorno.Delete(dispinfoSelectDataGridKoreanPorno);
                ChkKoreanPornoMoveFolder.IsChecked = false;
            }
            catch (Exception ex)
            {
                Debug.Write(ex);
                MessageBox.Show(ex.Message);
            }

            dgridKoreanPorno.Visibility = System.Windows.Visibility.Visible;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            dgridKoreanPorno.Visibility = System.Windows.Visibility.Visible;
        }

        private void OnPasteKoreanPornoPath(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            if (btn == null)
                return;

            if (btn.Name.IndexOf("Export") >= 0)
                txtKoreanPornoExportPath.Text = ClipBoardCommon.GetTextPath();
            else
                txtKoreanPornoPath.Text = ClipBoardCommon.GetTextPath();
        }

        private void MakeStorePath(object sender, RoutedEventArgs e)
        {
            if (Validation.GetHasError(txtKoreanPornoExportPath))
                return;

            //if (Validation.GetHasError(txtKoreanPornoExportStorePath))
            //    return;

            StoreData storeData = new StoreData();
            storeData.Label = txtKoreanPornoExportStorePath.Text;
            storeData.Name1 = txtKoreanPornoExportStorePath.Text;
            storeData.Path = txtKoreanPornoExportPath.Text;
            storeData.Type = "file";

            ColViewStore.Add(storeData);
        }

        private void OnMakersFilter(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                Button btn = sender as Button;
                if (btn.Content != null && (string)btn.Content == "AUTO")
                {
                    ColViewMaker.SetAutoOnly();
                    ColViewMaker.Execute();
                    return;
                }
                else
                {
                    chkKindOne.IsChecked = false;
                    chkKindTwo.IsChecked = false;
                    chkKindThree.IsChecked = false;
                    return;
                }
            }

            List<int> intList = new List<int>();

            if (chkKindOne.IsChecked != null)
                intList.Add(1);
            if (chkKindTwo.IsChecked != null)
                intList.Add(2);
            if (chkKindThree.IsChecked != null)
                intList.Add(3);

            ColViewMaker.SetCondition(intList.ToArray(), txtMakersSearch.Text);
            ColViewMaker.Execute();
        }

        private void btnClearActress_Click(object sender, RoutedEventArgs e)
        {
            txtTag.Text = "";
        }

        private void btnFileGenDeleteSelectFile_Click(object sender, RoutedEventArgs e)
        {
            if (dgridCheckExistFiles.SelectedItems == null || dgridCheckExistFiles.SelectedItems.Count <= 0)
                return;

            MessageBoxResult result = MessageBox.Show("削除しますか？", "削除確認", MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.Cancel)
                return;

            foreach(TargetFiles file in dgridCheckExistFiles.SelectedItems)
            {
                FileSystem.DeleteFile(
                    file.FileInfo.FullName,
                    UIOption.OnlyErrorDialogs,
                    RecycleOption.SendToRecycleBin);
            }

            btnFileGenSearch_Click(null, null);
        }

        private void btnFileGenSearch_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = txtFileGeneSearchText.Text;

            ColViewFileGeneTargetFiles.FilterSearchProductNumber = txtSearch.Text;
            ColViewFileGeneTargetFiles.Refresh();
        }

        private void tbtnFileGenHdUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (tbtnFileGenHdUpdate.IsChecked != null)
            {
                bool updateChecked = (bool)tbtnFileGenHdUpdate.IsChecked;

                if (updateChecked && txtbFileGenFileId.Text.Length <= 0)
                {
                    List<MovieFileContents> fileContentsList = ColViewMovieFileContents.MatchProductNumber(txtProductNumber.Text);

                    if (fileContentsList.Count > 0)
                    {
                        MovieFileContents fileContents = fileContentsList[0];
                        txtbFileGenFileId.Text = Convert.ToString(fileContents.Id);
                    }
                }
            }
        }

        private void btnFileGenClearFileId_Click(object sender, RoutedEventArgs e)
        {
            txtbFileGenFileId.Text = "";
            tbtnFileGenHdUpdate.IsChecked = false;
        }

        private void txtSearchTartgetFilename_TextChanged(object sender, TextChangedEventArgs e)
        {
            ColViewMovieImport.Filter(txtSearchTartgetFilename.Text);
        }

        private void tbtnFileGenModeFilenameOnly_Click(object sender, RoutedEventArgs e)
        {
            if (GetChecked(tbtnFileGenModeFilenameOnly))
                this.Background = new SolidColorBrush(Colors.LightGreen);
            else
                this.Background = new SolidColorBrush(Colors.White);
        }
        public bool GetChecked(ToggleButton myToggleButton)
        {
            bool b = false;
            if (myToggleButton.IsChecked != null)
                b = (bool)myToggleButton.IsChecked;

            return b;
        }

        private void dgridSelectTargetFilename_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MovieImportData data = dgridSelectTargetFilename.SelectedItem as MovieImportData;

            if (data == null)
                return;

            txtFileGenSelectFilename.Text = data.Filename;
        }

        private void btnFileGenLoad_Click(object sender, RoutedEventArgs e)
        {
            dgridCheckExistFiles.ItemsSource = ColViewFileGeneTargetFiles.ColViewListTargetFiles;
        }

        private string GetFirstName(string myThumbnail)
        {
            string[] arrThumbnails = myThumbnail.Split(' ');

            string name = "";
            if (arrThumbnails.Length > 1)
                name = arrThumbnails[0];
            else
                name = myThumbnail;

            return name;
        }

        private void btnFileGenExecute_Click(object sender, RoutedEventArgs e)
        {
            dispinfoSelectMovieImportData = GetImportDataFromUIElement();
            List<TargetFiles> files = GetSelectCheckExistFiles();

            if (txtFilenameGenerate.Text.Trim().Length <= 10)
            {
                MessageBox.Show("ファイル名が未設定です");
                return;
            }

            if (files.Count <= 0)
            {
                MessageBox.Show("対象のファイルが選択されていません");
                return;
            }
            else if (files.Count > 20)
            {
                MessageBox.Show("ファイルが20個以上選択されています");
                return;
            }

            bool tbtnChecked = (bool)(tbtnFileGenModeFilenameOnly.IsChecked == null) ? false : (bool)tbtnFileGenModeFilenameOnly.IsChecked;
            if (tbtnChecked)
            {
                MessageBox.Show("名前登録のみなので、登録できません");
                return;
            }

            tbtnChecked = (bool)(tbtnFileGenSplit.IsChecked == null) ? false: (bool)tbtnFileGenSplit.IsChecked;
            if (tbtnChecked)
            {
                if (files.Count <= 1)
                {
                    MessageBox.Show("ファイル分割なのに、1個しかファイルが選択されていません");
                    return;
                }
            }
            else
            {
                if (files.Count != 1)
                {
                    MessageBox.Show("ファイル分割ではないのに、複数個のファイルが選択されています");
                    return;
                }
            }

            string packageSource = Path.Combine(JpegStorePath, txtPackage.Text);
            string thumbnailSource = Path.Combine(JpegStorePath, GetFirstName(txtThumbnail.Text));

            // Validationチェック
            if (Validation.GetHasError(txtFilenameGenerate))
            {
                MessageBox.Show("有効なファイル名ではありません");
                return;
            }

            FilesRegisterService service = new FilesRegisterService(new DbConnection());

            // 動画のファイル名変更
            StoreData storeData = null;
            try
            {
                storeData = ColViewStore.GetMatchByPath(txtLabelPath.Text);
                dispinfoSelectMovieImportData.StoreLabel = storeData.Label;

                service.BasePath = txtBasePath.Text;
                service.DestFilename = txtFilenameGenerate.Text;
                service.LabelPath = txtLabelPath.Text;
                service.listSelectedFiles = files;

                // 選択したファイルのみを対象に内部プロパティへ設定
                service.SetSelectedOnlyFiles(files);
            }
            catch (Exception ex)
            {
                Debug.Write(ex);
                MessageBox.Show(ex.Message, "初期設定エラー");
                return;
            }

            try
            {
                string jpegExtension = ".jpg";
                string packageDest = Path.Combine(txtBasePath.Text, txtFilenameGenerate.Text + jpegExtension);
                string thumbnailDest = Path.Combine(txtBasePath.Text, txtFilenameGenerate.Text + "_th" + jpegExtension);

                // データベースへ登録用の情報を生成、登録
                service.SetDbMovieFilesInfo(dispinfoSelectMovieImportData, storeData);
                service.DbExport();

                // パッケージ画像のコピー
                File.Copy(packageSource, packageDest);

                // サムネイル画像のコピー
                File.Copy(thumbnailSource, thumbnailDest);

                // 動画の変換用の情報を生成する
                service.SetMovieActionInfo();

                // 動画ファイルの移動、日付コピー等の実行
                service.Execute();
            }
            catch (Exception exp)
            {
                Debug.Write(exp);
                MessageBox.Show(exp.Message);
                return;
            }
            finally
            {
                // 選択中のファイル一覧はクリアする（次の対象動画になってしまうので）
                foreach (TargetFiles file in dgridCheckExistFiles.ItemsSource)
                    file.IsSelected = false;

                // フィルターをクリアしないと再取得した直後に動作して不要なチェックが付いてしまう
                dgridDestFile.Items.Filter = null;
            }

            try
            {
                service.DeleteFiles(ColViewFileGeneTargetFiles.listTargetFiles);
            }
            catch (Exception ex)
            {
                MessageBox.Show("削除失敗 " + ex.Message);
            }

            ColViewMovieImport.Refresh();

            ClearUIElement();
            ColViewMovieImport.Refresh();
        }

        private void btnJumpUrl_Click(object sender, RoutedEventArgs e)
        {
            SearchResultData selData = (SearchResultData)cmbSearchResult.SelectedItem;

            if (selData != null)
            {
                System.Diagnostics.Process.Start(selData.Url);
            }
        }

        private void btnRefrectActress_Click(object sender, RoutedEventArgs e)
        {
            SearchResultData selData = (SearchResultData)cmbSearchResult.SelectedItem;

            if (selData != null)
                txtTag.Text = selData.Name;
        }

        private void btnPasteActressesClear_Click(object sender, RoutedEventArgs e)
        {
            txtTag.Text = "";
        }

        private void btnPasteActressesSearch_Click(object sender, RoutedEventArgs e)
        {
            if (txtTag.Text.Length <= 0)
            {
                txtResultRating.Text = "";
                return;
            }

            AvContentsService contentsService = new AvContentsService();
            txtResultRating.Text = contentsService.GetActressRating(txtTag.Text, new MySqlDbConnection());
        }
    }
}
