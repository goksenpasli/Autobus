using Autobus.Converter;
using Autobus.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
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
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                return null;
            }
            if (File.Exists(MainViewModel.xmldatapath))
            {
                return MainViewModel.xmldatapath.DeSerialize<Otobüs>().Araçlar.Araç;
            }
            _ = Directory.CreateDirectory(Path.GetDirectoryName(MainViewModel.xmldatapath));
            return new ObservableCollection<Araç>();
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
            _ = Directory.CreateDirectory(Path.GetDirectoryName(MainViewModel.xmldatapath));
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
                Tutar = sefer.SelectMany(z => z.Müşteri).Where(z => z.BiletÖdendi).Sum(z => z.BiletFiyat),
                ÜrünTutar = otobüs?.Sefer?.Where(z => z.KalkışZamanı.Month == sefer.Key).SelectMany(z => z.Müşteri).SelectMany(z => z.Sipariş).SelectMany(t => otobüs?.Ürünler.Ürün.Where(z => z.Id == t.ÜrünId)).Sum(z => z.ÜrünFiyat) ?? 0
            });
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
                    ChartBrush = (Brush)stringToBrushConverter.Convert(otobüs?.Aylar?.FirstOrDefault(z => z.Ad == tahsilat.Tarih)?.Renk, null, null, CultureInfo.CurrentCulture),
                    Description = tahsilat.Tarih,
                    ChartValue = (double)tahsilat.Tutar
                });
            }
            return data;
        }

        public static ObservableCollection<Marka> MarkalarıYükle()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                return null;
            }
            if (File.Exists(MainViewModel.xmldatapath))
            {
                return MainViewModel.xmldatapath.DeSerialize<Otobüs>().Markalar.Marka;
            }
            _ = Directory.CreateDirectory(Path.GetDirectoryName(MainViewModel.xmldatapath));
            return new ObservableCollection<Marka>();
        }

        public static string RandomColor()
        {
            return $"#{new Random(Guid.NewGuid().GetHashCode()).Next(0x1000000):X6}";
        }

        public static int RandomNumber()
        {
            return new Random(Guid.NewGuid().GetHashCode()).Next(1, int.MaxValue);
        }

        public static ObservableCollection<Sefer> SeferleriYükle()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                return null;
            }
            if (File.Exists(MainViewModel.xmldatapath))
            {
                return MainViewModel.xmldatapath.DeSerialize<Otobüs>().Sefer;
            }
            _ = Directory.CreateDirectory(Path.GetDirectoryName(MainViewModel.xmldatapath));
            return new ObservableCollection<Sefer>();
        }

        public static void Serialize<T>(this T dataToSerialize) where T : class
        {
            XmlSerializer serializer = new(typeof(T));
            using TextWriter stream = new StreamWriter(MainViewModel.xmldatapath);
            serializer.Serialize(stream, dataToSerialize);
        }

        public static ObservableCollection<Şöför> ŞoförleriYükle()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                return null;
            }
            if (File.Exists(MainViewModel.xmldatapath))
            {
                return MainViewModel.xmldatapath.DeSerialize<Otobüs>().Şöförler.Şöför;
            }
            _ = Directory.CreateDirectory(Path.GetDirectoryName(MainViewModel.xmldatapath));
            return new ObservableCollection<Şöför>();
        }

        public static ObservableCollection<Ürün> ÜrünleriYükle()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                return null;
            }
            if (File.Exists(MainViewModel.xmldatapath))
            {
                return MainViewModel.xmldatapath.DeSerialize<Otobüs>().Ürünler.Ürün;
            }
            _ = Directory.CreateDirectory(Path.GetDirectoryName(MainViewModel.xmldatapath));
            return new ObservableCollection<Ürün>();
        }

        private static readonly string[] ColorNames = typeof(Brushes).GetProperties(BindingFlags.Public | BindingFlags.Static).Select(propInfo => propInfo.Name).ToArray();
    }
}