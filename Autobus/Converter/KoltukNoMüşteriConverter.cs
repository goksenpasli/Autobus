using Autobus.Model;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Autobus.Converter
{
    public sealed class KoltukNoMüşteriConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is int koltukno && values[1] is Sefer seçilisefer)
            {
                return seçilisefer?.Müşteri?.FirstOrDefault(z => z.KoltukNo == koltukno);
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}