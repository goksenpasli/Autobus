using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Autobus.Converter
{
    public sealed class NumberToSplitterVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is int sayı) ? (sayı % 2 == 0) ? Visibility.Visible : Visibility.Collapsed : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}