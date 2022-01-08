using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Autobus.Model
{
    [XmlRoot(ElementName = "Şöförler")]
    public class Şöförler : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [XmlElement(ElementName = "Şöför")]
        public ObservableCollection<Şöför> Şöför { get; set; }
    }
}