using Autobus.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using static Extensions.GraphControl;

namespace Autobus.ViewModel
{
    public class ÖdemeYapılmayanKoltuklarViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Chart> GrafikVerileri { get; set; }

        public IEnumerable<Müşteri> ÖdemeYapmayanMüşteriler { get; set; }

        public IEnumerable<Tahsilat> Tahsilatlar { get; set; }
    }
}