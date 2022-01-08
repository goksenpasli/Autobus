using System.ComponentModel;
using System.Xml.Serialization;

namespace Autobus.Model
{
    [XmlRoot(ElementName = "Marka")]
    public class Marka : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [XmlAttribute(AttributeName = "Açıklama")]
        public string Açıklama { get; set; }

        [XmlAttribute(AttributeName = "Id")]
        public int Id { get; set; }
    }
}