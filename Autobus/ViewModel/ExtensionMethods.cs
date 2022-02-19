using Autobus.Converter;
using Autobus.Model;
using Autobus.Properties;
using Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using System.Xml.Serialization;
using static Extensions.GraphControl;

namespace Autobus.ViewModel
{
    public static class ExtensionMethods
    {
        public static readonly StringToBrushConverter stringToBrushConverter = new();

        public static ObservableCollection<Araç> AraçlarıYükle()
        {
            return DesignerProperties.GetIsInDesignMode(new DependencyObject())
                ? null
                : !File.Exists(MainViewModel.xmldatapath)
                ? new ObservableCollection<Araç>()
                : MainViewModel.xmldatapath.DeSerialize<Otobüs>().Araçlar.Araç;
        }

        public static IEnumerable<Tahsilat> AraçMasraflar(this Otobüs otobüs)
        {
            return otobüs?.Araçlar?.Araç?.GroupBy(sefer => sefer.Plaka).Select(sefer =>
            {
                AraçMasraf araçMasraf = new();
                araçMasraf.Açıklama = sefer.Key;
                araçMasraf.MasrafKarşılananTutar = sefer.SelectMany(z => z.Masraf).Where(z => z.Karşılandı).Sum(z => z.Tutar);
                araçMasraf.ToplamTutar = sefer.SelectMany(z => z.Masraf).Sum(z => z.Tutar);
                araçMasraf.Oran = araçMasraf.MasrafKarşılananTutar / araçMasraf.ToplamTutar;
                return araçMasraf;
            });
        }

        public static ObservableCollection<Aylar> AylarıYükle()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                return null;
            }
            if (File.Exists(MainViewModel.xmldatapath))
            {
                return MainViewModel.xmldatapath.DeSerialize<Otobüs>().Aylar;
            }
            ObservableCollection<Aylar> aylar = new();
            Random rnd = new(Guid.NewGuid().GetHashCode());
            foreach (string monthName in DateTimeFormatInfo.CurrentInfo.MonthNames.Take(12))
            {
                aylar.Add(new Aylar() { Ad = monthName, Renk = ColorNames[rnd.Next(0, ColorNames.Length)] });
            }
            return aylar;
        }

        public static IEnumerable<Tahsilat> BiletTahsilat(this Otobüs otobüs)
        {
            return otobüs?.Sefer?.Where(sefer => sefer.KalkışZamanı.Year == DateTime.Today.Year && !sefer.İptal).GroupBy(sefer => sefer.KalkışZamanı.Month).OrderBy(z => z.Key).Select(sefer => new Tahsilat()
            {
                Tarih = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(sefer.Key),
                ToplamTutar = sefer.SelectMany(z => z.Müşteri).Where(z => z.BiletÖdendi).Sum(z => z.BiletFiyat),
                ÜrünTutar = otobüs?.Sefer?.Where(z => z.KalkışZamanı.Month == sefer.Key).SelectMany(z => z.Müşteri).SelectMany(z => z.Sipariş).SelectMany(t => otobüs?.Ürünler.Ürün.Where(z => z.Id == t.ÜrünId)).Sum(z => z.ÜrünFiyat) ?? 0
            });
        }

        public static Bitmap BitmapChangeFormat(this Bitmap bitmap, System.Drawing.Imaging.PixelFormat format)
        {
            return bitmap.Clone(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), format);
        }

        public static T DeSerialize<T>(this string xmldatapath) where T : class, new()
        {
            XmlSerializer serializer = new(typeof(T));
            using StreamReader stream = new(xmldatapath);
            return serializer.Deserialize(stream) as T;
        }

        public static T DeSerialize<T>(this XElement xElement) where T : class, new()
        {
            XmlSerializer serializer = new(typeof(T));
            return serializer.Deserialize(xElement.CreateReader()) as T;
        }

        public static ObservableCollection<T> DeSerialize<T>(this IEnumerable<XElement> xElement) where T : class, new()
        {
            ObservableCollection<T> list = new();
            foreach (XElement element in xElement)
            {
                list.Add(element.DeSerialize<T>());
            }
            return list;
        }

        public static ObservableCollection<Chart> GrafikVerileri(this Otobüs otobüs)
        {
            ObservableCollection<Chart> data = new();
            foreach (Tahsilat tahsilat in otobüs.BiletTahsilat())
            {
                data.Add(new Chart()
                {
                    ChartBrush = (System.Windows.Media.Brush)stringToBrushConverter.Convert(otobüs?.Aylar?.FirstOrDefault(z => z.Ad == tahsilat.Tarih)?.Renk, null, null, CultureInfo.CurrentCulture),
                    Description = tahsilat.Tarih,
                    ChartValue = (double)tahsilat.ToplamTutar
                });
            }
            return data;
        }

        public static ObservableCollection<İade> İadeleriYükle()
        {
            return DesignerProperties.GetIsInDesignMode(new DependencyObject())
                ? null
                : File.Exists(MainViewModel.xmldatapath)
                ? MainViewModel.xmldatapath.DeSerialize<Otobüs>().İadeler.İade
                : new ObservableCollection<İade>();
        }

        public static ObservableCollection<Marka> MarkalarıYükle()
        {
            return DesignerProperties.GetIsInDesignMode(new DependencyObject())
                ? null
                : File.Exists(MainViewModel.xmldatapath)
                ? MainViewModel.xmldatapath.DeSerialize<Otobüs>().Markalar.Marka
                : new ObservableCollection<Marka>();
        }

        public static string RandomColor()
        {
            return $"#{new Random(Guid.NewGuid().GetHashCode()).Next(0x1000000):X6}";
        }

        public static int RandomNumber()
        {
            return new Random(Guid.NewGuid().GetHashCode()).Next(1, int.MaxValue);
        }

        public static string ResimYükle(this string file, double en, double boy, bool webp = false)
        {
            string filename = Guid.NewGuid() + Path.GetExtension(file);
            BitmapImage image = new(new Uri(file));
            if (webp && Settings.Default.WebpEncode)
            {
                filename = Path.ChangeExtension(filename, ".webp");
                File.WriteAllBytes($"{Path.GetDirectoryName(MainViewModel.xmldatapath)}\\{filename}", image.Resize(en, boy).ToTiffJpegByteArray(Extensions.ExtensionMethods.Format.Jpg).WebpEncode(Settings.Default.WebpQuality));
            }
            else
            {
                File.WriteAllBytes($"{Path.GetDirectoryName(MainViewModel.xmldatapath)}\\{filename}", image.Resize(en, boy).ToTiffJpegByteArray(Extensions.ExtensionMethods.Format.Jpg));
            }
            return filename;
        }

        public static ObservableCollection<Sefer> SeferleriYükle()
        {
            return DesignerProperties.GetIsInDesignMode(new DependencyObject())
                ? null
                : File.Exists(MainViewModel.xmldatapath)
                ? MainViewModel.xmldatapath.DeSerialize<Otobüs>().Sefer
                : new ObservableCollection<Sefer>();
        }

        public static void Serialize<T>(this T dataToSerialize) where T : class
        {
            XmlSerializer serializer = new(typeof(T));
            using TextWriter stream = new StreamWriter(MainViewModel.xmldatapath);
            serializer.Serialize(stream, dataToSerialize);
        }

        public static ObservableCollection<Şöför> ŞoförleriYükle()
        {
            return DesignerProperties.GetIsInDesignMode(new DependencyObject())
                ? null
                : File.Exists(MainViewModel.xmldatapath)
                ? MainViewModel.xmldatapath.DeSerialize<Otobüs>().Şöförler.Şöför
                : new ObservableCollection<Şöför>();
        }

        public static BitmapImage ToBitmapImage(this Image bitmap, ImageFormat format, double decodeheight = 0)
        {
            if (bitmap != null)
            {
                using MemoryStream memoryStream = new();
                bitmap.Save(memoryStream, format);
                memoryStream.Position = 0;
                BitmapImage image = new();
                image.BeginInit();
                if (decodeheight != 0)
                {
                    image.DecodePixelHeight = bitmap.Height > (int)decodeheight ? (int)decodeheight : bitmap.Height;
                }

                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = memoryStream;
                image.EndInit();
                bitmap.Dispose();
                if (!image.IsFrozen && image.CanFreeze)
                {
                    image.Freeze();
                }
                return image;
            }

            return null;
        }

        public static ObservableCollection<Ürün> ÜrünleriYükle()
        {
            return DesignerProperties.GetIsInDesignMode(new DependencyObject())
                ? null
                : File.Exists(MainViewModel.xmldatapath)
                ? MainViewModel.xmldatapath.DeSerialize<Otobüs>().Ürünler.Ürün
                : new ObservableCollection<Ürün>();
        }

        public static ImageSource WebpDecode(this string file)
        {
            using WebP webp = new();
            return webp.Load(file).ToBitmapImage(ImageFormat.Jpeg);
        }

        public static byte[] WebpEncode(this byte[] resim, int kalite)
        {
            try
            {
                using WebP webp = new();
                using MemoryStream ms = new(resim);
                using Bitmap bmp = Image.FromStream(ms) as Bitmap;
                resim = null;
                return webp.EncodeLossy(bmp, kalite);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, App.Current.MainWindow.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public static byte[] WebpEncode(this string resimdosyayolu, int kalite)
        {
            try
            {
                using WebP webp = new();
                using Bitmap bmp = new(resimdosyayolu);
                return bmp.PixelFormat is System.Drawing.Imaging.PixelFormat.Format24bppRgb or System.Drawing.Imaging.PixelFormat.Format32bppArgb ? webp.EncodeLossy(bmp, kalite) : webp.EncodeLossy(bmp.BitmapChangeFormat(System.Drawing.Imaging.PixelFormat.Format24bppRgb), kalite);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, App.Current.MainWindow.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        private static readonly string[] ColorNames = typeof(System.Windows.Media.Brushes).GetProperties(BindingFlags.Public | BindingFlags.Static).Select(propInfo => propInfo.Name).Where(z => z != "Transparent").ToArray();
    }
}