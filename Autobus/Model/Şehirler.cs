using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Autobus.Model
{
    [XmlRoot(ElementName = "Şehirler")]
    public class Şehirler : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [XmlElement(ElementName = "Şehir")]
        public ObservableCollection<Şehir> Şehir { get; set; }
    }
}