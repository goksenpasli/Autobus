using System.ComponentModel;
using System.Xml.Serialization;

namespace Autobus.Model
{
    [XmlRoot(ElementName = "Masraf")]
    public class Masraf : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [XmlAttribute(AttributeName = "Açıklama")]
        public string Açıklama { get; set; }

        [XmlAttribute(AttributeName = "Id")]
        public int Id { get; set; }

        [XmlAttribute(AttributeName = "Karşılandı")]
        public bool Karşılandı { get; set; }

        [XmlAttribute(AttributeName = "SeferId")]
        public int SeferId { get; set; }

        [XmlAttribute(AttributeName = "Tutar")]
        public double Tutar { get; set; }
    }
}