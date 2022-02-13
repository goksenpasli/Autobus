using System.ComponentModel;

namespace Autobus.Model
{
    public class AraçMasraf : Tahsilat
    {
        public string Açıklama { get; set; }

        public double MasrafKarşılananTutar { get; set; }

        public double Oran { get; set; }
    }

    public class Tahsilat : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Tarih { get; set; }

        public double ToplamTutar { get; set; }

        public double ÜrünTutar { get; set; }
    }
}