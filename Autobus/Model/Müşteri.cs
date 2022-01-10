using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Autobus.Model
{
    [XmlRoot(ElementName = "Müşteri")]
    public class Müşteri : Ortak
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
    }
}