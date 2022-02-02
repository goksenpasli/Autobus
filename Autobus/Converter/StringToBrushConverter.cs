using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Autobus.Converter
{
    public class StringToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DesignerProperties.GetIsInDesignMode(new DependencyObject())
                ? null
                : (value is string renk) ? !string.IsNullOrEmpty(renk) ? new BrushConverter().ConvertFromString(renk) : null : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as SolidColorBrush)?.ToString();
        }
    }
}