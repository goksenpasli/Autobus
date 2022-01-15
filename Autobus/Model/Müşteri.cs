using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Autobus.Model
{
    [XmlRoot(ElementName = "Müşteri")]
    public class Müşteri : Ortak,IDataErrorInfo
    {
        public new event PropertyChangedEventHandler PropertyChanged;

        [XmlAttribute(AttributeName = "BiletFiyat")]
        public double BiletFiyat { get; set; }

        [XmlAttribute(AttributeName = "BiletÖdendi")]
        public bool BiletÖdendi { get; set; }

        [XmlAttribute(AttributeName = "Cinsiyet")]
        public int Cinsiyet { get; set; } = -1;

        [XmlAttribute(AttributeName = "Id")]
        public int Id { get; set; }

        [XmlAttribute(AttributeName = "KoltukDolu")]
        public bool KoltukDolu { get; set; }

        [XmlAttribute(AttributeName = "KoltukNo")]
        public int KoltukNo { get; set; }

        [XmlElement(ElementName = "Sipariş")]
        public ObservableCollection<Sipariş> Sipariş { get; set; } = new();

        public string Error => string.Empty;

        public string this[string columnName] => columnName switch
        {
            "KoltukNo" when KoltukNo == 0 => "Koltuk Seçimi Yapın.",
            _ => null
        };
    }
}