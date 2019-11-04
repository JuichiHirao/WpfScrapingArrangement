using System;
using System.Globalization;
using System.Windows.Controls;
using System.IO;
using WpfScrapingArrangement.collection;

namespace WpfScrapingArrangement
{
    class TextBoxFilePathRule : ValidationRule
    {

        public override ValidationResult Validate( object value, CultureInfo cultureInfo )
        {
            string parameter = "";

            try
            {
                if( ((string) value).Length > 0 )
                {
                    if (!Directory.Exists(((string)value)))
                        return new ValidationResult(false, "存在しないパスが指定されています");
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
    class TextBoxStorePathRule : ValidationRule
    {

        StoreCollection storeCol = new StoreCollection();

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string parameter = "";

            try
            {
                if (((string)value).Length > 0)
                {
                    storeCol.GetMatchByLabel((string)value);

                    parameter = (string)value;
                }
            }
            catch (Exception e)
            {
                return new ValidationResult(false, "存在しないパスが指定されています");
            }

            return new ValidationResult(true, null);
        }
    }
}
