using System.ComponentModel;
using System.Xml.Serialization;

namespace Autobus.Model
{
    [XmlRoot(ElementName = "Ürün")]
    public class Ürün : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [XmlAttribute(AttributeName = "Id")]
        public int Id { get; set; }

        [XmlAttribute(AttributeName = "ÜrünFiyat")]
        public double ÜrünFiyat { get; set; }

        [XmlAttribute(AttributeName = "ÜrünAçıklama")]
        public string ÜrünAçıklama { get; set; }
    }
}