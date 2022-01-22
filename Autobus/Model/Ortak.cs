using System.ComponentModel;
using System.Xml.Serialization;

namespace Autobus.Model
{
    public abstract class Ortak : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [XmlAttribute(AttributeName = "Ad")]
        public string Ad { get; set; }

        [XmlAttribute(AttributeName = "Adres")]
        public string Adres { get; set; }

        [XmlAttribute(AttributeName = "Resim")]
        public string Resim { get; set; }

        [XmlAttribute(AttributeName = "Soyad")]
        public string Soyad { get; set; }

        [XmlAttribute(AttributeName = "Telefon")]
        public string Telefon { get; set; }
    }
}