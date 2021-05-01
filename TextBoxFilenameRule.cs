using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows.Controls;

namespace WpfScrapingArrangement
{
    class TextBoxFilenameRule : ValidationRule
    {
        public override ValidationResult Validate( object value, CultureInfo cultureInfo )
        {
            try
            {
                if( ((string) value).Length > 0 )
                {
                    string tempPath = Path.GetTempPath();
                    string filename = ((string)value);
                    string filePathname = Path.Combine(tempPath, filename);
                    char[] invalidChars = Path.GetInvalidFileNameChars();
                    string invalidStr = "";

                    foreach (char invalidChar in invalidChars)
                    {
                        if (filename.IndexOf(invalidChar) >= 0)
                        {
                            Debug.Print("使用できない文字があります [" + invalidChar + "]" + filename);
                            invalidStr = invalidStr + Convert.ToString(invalidChar);
                        }
                    }

                    if (invalidStr.Length > 0)
                    {
                        return new ValidationResult(false, "不正な文字[" + invalidStr + "]が含まれている、有効ではないファイル名です");
                    }

                    try
                    {
                        //GetFullPathメソッドに渡して、例外がスローされるか確かめる
                        System.IO.Path.GetFullPath(filePathname);
                    }
                    catch (ArgumentException e)
                    {
                        //ファイル名に不正な文字が含まれている場合など
                        Debug.Write(e);
                        return new ValidationResult(false, "不正な文字が含まれている、有効ではないファイル名です");
                    }
                    catch (System.Security.SecurityException e)
                    {
                        //アクセスできない場合
                        Debug.Write(e);
                        return new ValidationResult(false, "アクセスできません、有効ではないファイル名です");
                    }
                    catch (NotSupportedException e)
                    {
                        //ボリューム識別子以外に「:」がある場合
                        Debug.Write(e);
                        return new ValidationResult(false, ":があります、有効ではないファイル名です");
                    }
                    catch (System.IO.PathTooLongException e)
                    {
                        //パス名が長すぎる場合
                        Debug.Write(e);
                        return new ValidationResult(false, "長すぎます、有効ではないファイル名です");
                    }
                    try
                    {
                        StreamWriter toucher = new StreamWriter(filename);
                        toucher.Close();
                    }
                    catch (Exception e)
                    {
                        Debug.Write(e);
                        return new ValidationResult(false, "有効ではないファイル名です " + filename.Length + "");
                    }
                }
            }
            catch( Exception e )
            {
                return new ValidationResult( false, "Illegal characters or " + e.Message );
            }

            return new ValidationResult( true, null );
        }
    }
}