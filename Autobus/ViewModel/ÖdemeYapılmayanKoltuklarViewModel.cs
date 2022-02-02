using Autobus.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;
using static Extensions.GraphControl;

namespace Autobus.ViewModel
{
    public class ÖdemeYapılmayanKoltuklarViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public IEnumerable<Müşteri> ÖdemeYapmayanMüşteriler { get; set; }

        public IEnumerable<Tahsilat> Tahsilatlar { get; set; }

        public ObservableCollection<Chart> GrafikVerileri { get; set; }
    }

    public class Tahsilat : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Tarih { get; set; }

        public double Tutar { get; set; }
    }
}