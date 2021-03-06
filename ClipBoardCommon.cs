﻿using System.IO;
using System.Windows;

namespace WpfScrapingArrangement
{
    public class ClipBoardCommon
    {
        public static string GetText()
        {
            System.Windows.IDataObject data = Clipboard.GetDataObject();

            string ClipboardText = "";
            if (data.GetDataPresent(DataFormats.Text))
            {
                ClipboardText = (string)data.GetData(DataFormats.Text);
                // クリップボードのテキストを改行毎に配列に設定
                string[] ClipBoardList = ClipboardText.Split('\n');

                ClipboardText = string.Join(" ", ClipBoardList);
                /*
                foreach (string text in ClipBoardList)
                {
                    if (text.Trim().Length > 0)
                        ClipboardText = text;
                }
                 */
            }
            if (data.GetDataPresent(DataFormats.FileDrop))
            {
                foreach (string file in (string[])data.GetData(DataFormats.FileDrop))
                {
                    FileInfo fileinfo = new FileInfo(file);
                    DirectoryInfo dirinfo = new DirectoryInfo(file);

                    if (fileinfo.Exists || dirinfo.Exists)
                    {
                        ClipboardText = fileinfo.Name;
                        break;
                    }
                }
            }

            return ClipboardText;
        }
        public static string GetTextPath()
        {
            System.Windows.IDataObject data = Clipboard.GetDataObject();

            string ClipboardText = "";
            if (data.GetDataPresent(DataFormats.Text))
            {
                ClipboardText = (string)data.GetData(DataFormats.Text);
                // クリップボードのテキストを改行毎に配列に設定
                string[] ClipBoardList = ClipboardText.Split('\n');

                foreach (string file in ClipBoardList)
                {
                    FileInfo fileinfo = new FileInfo(file);
                    DirectoryInfo dirinfo = new DirectoryInfo(file);

                    if (fileinfo.Exists || dirinfo.Exists)
                    {
                        ClipboardText = fileinfo.FullName;
                        break;
                    }
                }
            }

            if (data.GetDataPresent(DataFormats.FileDrop))
            {
                foreach (string file in (string[])data.GetData(DataFormats.FileDrop))
                {
                    FileInfo fileinfo = new FileInfo(file);
                    DirectoryInfo dirinfo = new DirectoryInfo(file);

                    if (fileinfo.Exists || dirinfo.Exists)
                    {
                        ClipboardText = fileinfo.FullName;
                        break;
                    }
                }
            }

            return ClipboardText;
        }
    }
}