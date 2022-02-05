using PropertyChanged;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;

namespace Autobus.Model
{
    [XmlRoot(ElementName = "Sefer")]
    public class Sefer : INotifyPropertyChanged, IDataErrorInfo
    {
        static Sefer()
        {
            İllerListe = App.Current.TryFindResource("İller") as ArrayList;
        }

        public Sefer()
        {
            VarışZamanı = KalkışZamanı.AddHours(TahminiSüre).AddHours(KalkışSaat);
            PropertyChanged += Sefer_PropertyChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [XmlAttribute(AttributeName = "AraçId")]
        public int AraçId { get; set; } = -1;

        [DependsOn("KalkışŞehirId", "VarışŞehirId")]
        public string Başlık => (KalkışŞehirId == 0 || VarışŞehirId == 0) ? string.Empty : $"{((DictionaryEntry)İllerListe[KalkışŞehirId - 1]).Value?.ToString()[0]}{((DictionaryEntry)İllerListe[VarışŞehirId - 1]).Value?.ToString()[0]}";

        [XmlAttribute(AttributeName = "BiletTutarı")]
        public double BiletTutarı { get; set; } = 0;

        public string Error => string.Empty;

        [XmlAttribute(AttributeName = "Id")]
        public int Id { get; set; }

        [XmlIgnore]
        public double İlaveSüre { get; set; }

        [XmlAttribute(AttributeName = "İptal")]
        public bool İptal { get; set; }

        [XmlIgnore]
        public double KalkışSaat { get; set; }

        [XmlIgnore]
        public double KalkışSaatGüncelle { get; set; }

        [XmlAttribute(AttributeName = "KalkışŞehirId")]
        public short KalkışŞehirId { get; set; } = -1;

        [DependsOn("VarışZamanı")]
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

        [XmlAttribute(AttributeName = "Renk")]
        public string Renk { get; set; }

        [XmlAttribute(AttributeName = "ŞöförId")]
        public int ŞöförId { get; set; }

        [XmlIgnore]
        [DependsOn("Mesafe")]
        public double TahminiSüre { get; set; }

        [XmlAttribute(AttributeName = "TarihDeğiştirildi")]
        public bool TarihDeğiştirildi { get; set; }

        [XmlAttribute(AttributeName = "VarışŞehirId")]
        public short VarışŞehirId { get; set; } = -1;

        [DependsOn("KalkışZamanı")]
        [XmlAttribute(AttributeName = "VarışZamanı")]
        public DateTime VarışZamanı { get; set; }

        public string this[string columnName] => columnName switch
        {
            "VarışZamanı" when VarışZamanı < KalkışZamanı => "Varış Zamanı Kalkış Zamanından Önce Olmaz.",
            "KalkışZamanı" when VarışZamanı < KalkışZamanı => "Varış Zamanı Kalkış Zamanından Önce Olmaz.",
            _ => null
        };

        private static readonly ArrayList İllerListe;

        private int mesafe;

        private void Sefer_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is "Mesafe")
            {
                TahminiSüre = Mesafe > 0 ? Math.Round(Mesafe / Properties.Settings.Default.OrtalamaHız, 2) : 0;
            }
            if (e.PropertyName is "TahminiSüre" or "KalkışZamanı")
            {
                VarışZamanı = KalkışZamanı.AddHours(TahminiSüre).AddHours(KalkışSaat);
            }
            if (e.PropertyName is "KalkışSaat")
            {
                KalkışZamanı = KalkışZamanı.AddHours(KalkışSaat);
                KalkışSaat = 0;
            }

            if (e.PropertyName is "İlaveSüre")
            {
                VarışZamanı = KalkışZamanı.AddHours(İlaveSüre);
                TarihDeğiştirildi = true;
            }
            if (e.PropertyName is "KalkışSaatGüncelle")
            {
                KalkışZamanı = KalkışZamanı.AddHours(KalkışSaatGüncelle);
                KalkışSaatGüncelle = 0;
                TarihDeğiştirildi = true;
            }
        }
    }
}