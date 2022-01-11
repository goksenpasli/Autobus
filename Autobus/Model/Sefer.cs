using PropertyChanged;
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
        public int AraçId { get; set; } = -1;

        [XmlAttribute(AttributeName = "BiletTutarı")]
        public double BiletTutarı { get; set; } = 1;

        [XmlAttribute(AttributeName = "Id")]
        public int Id { get; set; }

        [XmlIgnore]
        public TimeSpan KalkışSaat { get; set; }

        [XmlAttribute(AttributeName = "KalkışŞehirId")]
        public int KalkışŞehirId { get; set; } = -1;

        [XmlAttribute(AttributeName = "KalkışZamanı")]
        public DateTime KalkışZamanı { get; set; } = DateTime.Today;

        [XmlIgnore]
        [DependsOn("KalkışŞehirId", "VarışŞehirId")]
        public int Mesafe
        {
            get => (KalkışŞehirId > 0 && VarışŞehirId > 0) ? ViewModel.Mesafe.Mesafeler[KalkışŞehirId - 1, VarışŞehirId - 1] : 0;
            set => mesafe = value;
        }

        [XmlElement(ElementName = "Müşteri")]
        public ObservableCollection<Müşteri> Müşteri { get; set; } = new();

        [XmlAttribute(AttributeName = "ŞöförId")]
        public int ŞöförId { get; set; }

        [XmlIgnore]
        [DependsOn("Mesafe")]
        public double TahminiSüre
        {
            get => Mesafe > 0 ? Math.Round(Mesafe / Properties.Settings.Default.OrtalamaHız, 2) : 0;
            set => tahminiSüre = value;
        }

        [XmlAttribute(AttributeName = "Tamamlandı")]
        public bool Tamamlandı { get; set; }

        [XmlAttribute(AttributeName = "VarışŞehirId")]
        public int VarışŞehirId { get; set; } = -1;

        [XmlAttribute(AttributeName = "VarışZamanı")]
        public DateTime VarışZamanı { get; set; }

        private int mesafe;

        private double tahminiSüre;

        private void Sefer_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is "KalkışZamanı" or "TahminiSüre" or "KalkışSaat")
            {
                VarışZamanı = KalkışZamanı.AddHours(TahminiSüre).AddTicks(KalkışSaat.Ticks);
            }
        }
    }
}