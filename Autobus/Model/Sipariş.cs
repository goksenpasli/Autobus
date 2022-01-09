using System.ComponentModel;
using System.Xml.Serialization;

namespace Autobus.Model
{
    [XmlRoot(ElementName = "Sipariş")]
    public class Sipariş : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [XmlAttribute(AttributeName = "Id")]
        public int Id { get; set; }

        [XmlAttribute(AttributeName = "ÜrünId")]
        public int ÜrünId { get; set; }
    }
}