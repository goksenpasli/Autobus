﻿using PropertyChanged;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;

namespace Autobus.Model
{
    [XmlRoot(ElementName = "Araç")]
    public class Araç : INotifyPropertyChanged, IDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [XmlAttribute(AttributeName = "Aktif")]
        public bool Aktif { get; set; } = true;

        [XmlAttribute(AttributeName = "BoyKoltukSayısı")]
        [DependsOn("KoltukSayısı", "BölmeSayısı")]
        public int BoyKoltukSayısı
        {
            get => KoltukSayısı / BölmeSayısı;
            set => boyKoltukSayısı = value;
        }

        [XmlAttribute(AttributeName = "BölmeSayısı")]
        [DependsOn("KoltukSayısı")]
        public int BölmeSayısı { get; set; } = 1;

        public string Error => string.Empty;

        [XmlAttribute(AttributeName = "Id")]
        public int Id { get; set; }

        [XmlIgnore]
        public IEnumerable<int> KoltukÖnizlemeListe => Enumerable.Range(1, KoltukSayısı);

        [XmlAttribute(AttributeName = "KoltukSayısı")]
        [DependsOn("BölmeSayısı")]
        public int KoltukSayısı { get; set; }

        [XmlAttribute(AttributeName = "MarkaId")]
        public int MarkaId { get; set; }

        [XmlElement(ElementName = "Masraf")]
        public ObservableCollection<Masraf> Masraf { get; set; } = new();

        [XmlAttribute(AttributeName = "Plaka")]
        public string Plaka { get; set; }

        [XmlAttribute(AttributeName = "Resim")]
        public string Resim { get; set; }

        public string this[string columnName] => columnName switch
        {
            "BölmeSayısı" when KoltukSayısı % BölmeSayısı != 0 => "Bölme İşlemi Tam Çıkmıyor.",
            "KoltukSayısı" when KoltukSayısı % BölmeSayısı != 0 => "Bölme İşlemi Tam Çıkmıyor.",
            _ => null
        };

        private int boyKoltukSayısı;
    }
}