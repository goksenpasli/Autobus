using Autobus.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Autobus.ViewModel
{
    public static class ExtensionMethods
    {
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

        public static int RandomNumber()
        {
            return new Random(Guid.NewGuid().GetHashCode()).Next(1, int.MaxValue);
        }
        public static string RandomColor()
        {
            return $"#{new Random(Guid.NewGuid().GetHashCode()).Next(0x1000000):X6}";
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
    }
}