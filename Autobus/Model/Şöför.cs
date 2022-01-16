using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Autobus.Model
{
    [XmlRoot(ElementName = "Şöför")]
    public class Şöför : Ortak
    {
        public new event EventHandler<PropertyChangedEventArgs> PropertyChanged;

        [XmlAttribute(AttributeName = "Etkin")]
        public bool Etkin { get; set; }

        [XmlAttribute(AttributeName = "Id")]
        public int Id { get; set; }
    }
}