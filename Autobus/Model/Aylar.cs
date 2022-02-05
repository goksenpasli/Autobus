using System.ComponentModel;
using System.Xml.Serialization;

namespace Autobus.Model
{
    public class Aylar : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [XmlAttribute(AttributeName = "Ad")]
        public string Ad { get; set; }

        [XmlAttribute(AttributeName = "Renk")]
        public string Renk { get; set; }
    }
}