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
            string parameter = "";

            try
            {
                if( ((string) value).Length > 0 )
                {
                    string tempPath = Path.GetTempPath();
                    string filename = Path.Combine(tempPath, (string)value);

                    try
                    {
                        StreamWriter toucher = new StreamWriter(filename);
                        toucher.Close();
                    }
                    catch (Exception e)
                    {
                        Debug.Write(e);
                        return new ValidationResult(false, "有効ではないファイル名です");
                    }
                    parameter = (string) value;
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