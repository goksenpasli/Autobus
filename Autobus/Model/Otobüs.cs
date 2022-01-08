using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Autobus.Model
{
    [XmlRoot(ElementName = "Otobüs")]
    public class Otobüs : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [XmlElement(ElementName = "Araçlar")]
        public Araçlar Araçlar { get; set; }

        [XmlElement(ElementName = "Markalar")]
        public Markalar Markalar { get; set; }

        [XmlElement(ElementName = "Sefer")]
        public ObservableCollection<Sefer> Sefer { get; set; }

        [XmlElement(ElementName = "Şehirler")]
        public Şehirler Şehirler { get; set; }

        [XmlElement(ElementName = "Şöförler")]
        public Şöförler Şöförler { get; set; }
    }
}