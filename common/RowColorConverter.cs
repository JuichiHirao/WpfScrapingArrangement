using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace WpfScrapingArrangement.common
{
    class RowColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (Boolean.Parse(value.ToString()))
                return new LinearGradientBrush(Colors.LightGray, Colors.LightGray, 45);

            return new LinearGradientBrush(Colors.White, Colors.White, 45);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
