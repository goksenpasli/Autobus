using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Autobus.Model
{
    [XmlRoot(ElementName = "İade")]
    public class İade : Ortak, INotifyPropertyChanged
    {
        public new event EventHandler<PropertyChangedEventArgs> PropertyChanged;

        [XmlAttribute(AttributeName = "Id")]
        public int Id { get; set; }

        [XmlAttribute(AttributeName = "SeferId")]
        public int SeferId { get; set; }

        [XmlAttribute(AttributeName = "Tutar")]
        public double Tutar { get; set; }
    }
}