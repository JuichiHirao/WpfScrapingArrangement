using System;
using System.Globalization;
using System.Windows.Controls;

namespace WpfScrapingArrangement
{
    class TextBoxDateRule : ValidationRule
    {

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null || ((string)value).Length <= 0)
                return new ValidationResult(false, "datetime required ");

            DateTime dt;
            try
            {
                dt = Convert.ToDateTime(value);
            }
            catch (Exception)
            {
                return new ValidationResult(false, "Illegal datetime format ");
            }

            return new ValidationResult(true, null);
        }
    }
}