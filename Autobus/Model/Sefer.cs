using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;

namespace Autobus.Model
{
    [XmlRoot(ElementName = "Sefer")]
    public class Sefer : INotifyPropertyChanged
    {
        public Sefer()
        {
            VarışZamanı = KalkışZamanı.AddHours(TahminiSüre).AddTicks(KalkışSaat.Ticks);
            PropertyChanged += Sefer_PropertyChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [XmlAttribute(AttributeName = "AraçId")]
        public int AraçId { get; set; }

        [XmlAttribute(AttributeName = "BiletTutarı")]
        public double BiletTutarı { get; set; }

        [XmlAttribute(AttributeName = "Id")]
        public int Id { get; set; }

        [XmlIgnore]
        public TimeSpan KalkışSaat { get; set; }

        [XmlAttribute(AttributeName = "KalkışŞehirId")]
        public int KalkışŞehirId { get; set; }

        [XmlAttribute(AttributeName = "KalkışZamanı")]
        public DateTime KalkışZamanı { get; set; } = DateTime.Today;

        [XmlElement(ElementName = "Müşteri")]
        public ObservableCollection<Müşteri> Müşteri { get; set; } = new();

        [XmlAttribute(AttributeName = "ŞöförId")]
        public int ŞöförId { get; set; }

        [XmlIgnore]
        public double TahminiSüre { get; set; }

        [XmlAttribute(AttributeName = "Tamamlandı")]
        public bool Tamamlandı { get; set; }

        [XmlAttribute(AttributeName = "VarışŞehirId")]
        public int VarışŞehirId { get; set; }

        [XmlAttribute(AttributeName = "VarışZamanı")]
        public DateTime VarışZamanı { get; set; }

        private void Sefer_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is "KalkışZamanı" or "TahminiSüre" or "KalkışSaat")
            {
                VarışZamanı = KalkışZamanı.AddHours(TahminiSüre).AddTicks(KalkışSaat.Ticks);
            }
        }
    }
}