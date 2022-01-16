using Autobus.Model;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Autobus.Converter
{
    public sealed class KoltukNoVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is int koltukno && values[1] is Sefer seçilisefer && values[2] is int müşterikoltuk)
            {
                return seçilisefer?.Müşteri.Any(z => z.KoltukNo == koltukno) == true ? Visibility.Collapsed : Visibility.Visible;
            }
            return Visibility.Visible;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}