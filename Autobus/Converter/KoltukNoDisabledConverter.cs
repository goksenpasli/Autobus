using Autobus.Model;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Autobus.Converter
{
    public sealed class KoltukNoDisabledConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is int koltukno && values[1] is Sefer seçilisefer && values[2] is int müşterikoltuk)
            {
                return seçilisefer?.Müşteri.Any(z => z.KoltukNo == koltukno) != true;
            }
            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}