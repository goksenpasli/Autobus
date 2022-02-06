using System.ComponentModel;

namespace Autobus.Model
{
    public class Tahsilat : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Tarih { get; set; }

        public double Tutar { get; set; }

        public double ÜrünTutar { get; set; }
    }
}