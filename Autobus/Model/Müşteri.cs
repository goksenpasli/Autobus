using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Autobus.Model
{
    [XmlRoot(ElementName = "Müşteri")]
    public class Müşteri : Ortak, IDataErrorInfo
    {
        public new event EventHandler<PropertyChangedEventArgs> PropertyChanged;

        [XmlAttribute(AttributeName = "BiletFiyat")]
        public double BiletFiyat { get; set; }

        [XmlAttribute(AttributeName = "BiletÖdendi")]
        public bool BiletÖdendi { get; set; }

        [XmlAttribute(AttributeName = "Cinsiyet")]
        public int Cinsiyet { get; set; } = -1;

        public string Error => string.Empty;

        [XmlAttribute(AttributeName = "Id")]
        public int Id { get; set; }

        [XmlAttribute(AttributeName = "KoltukDolu")]
        public bool KoltukDolu { get; set; }

        [XmlAttribute(AttributeName = "KoltukNo")]
        public int KoltukNo { get; set; }

        [XmlAttribute(AttributeName = "SeferId")]
        public int SeferId { get; set; }

        [XmlElement(ElementName = "Sipariş")]
        public ObservableCollection<Sipariş> Sipariş { get; set; } = new();

        public string this[string columnName] => columnName switch
        {
            "Telefon" when Telefon is null || Telefon.Length < 10 => "Telefon Numarasını Eksiksiz Girin.",
            "KoltukNo" when KoltukNo == 0 => "Koltuk Seçimi Yapın.",
            _ => null
        };
    }
}