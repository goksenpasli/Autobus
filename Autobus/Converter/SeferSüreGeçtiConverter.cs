using System;
using System.Globalization;
using System.Windows.Data;

namespace Autobus.Converter
{
    public sealed class SeferSüreGeçtiConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is DateTime varıştarihi && varıştarihi <= DateTime.Now;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}