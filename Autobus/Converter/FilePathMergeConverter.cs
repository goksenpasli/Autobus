using Autobus.ViewModel;
using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Autobus.Converter
{
    public sealed class FilePathMergeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                if (value is string filename)
                {
                    string file = $"{Path.GetDirectoryName(MainViewModel.xmldatapath)}\\{filename}";
                    if (File.Exists(file))
                    {
                        return string.Equals(Path.GetExtension(file), ".webp", StringComparison.OrdinalIgnoreCase) ? file.WebpDecode() : file;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value as BitmapSource;
        }
    }
}