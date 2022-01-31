using Autobus.Model;
using System.Collections.Generic;
using System.ComponentModel;

namespace Autobus.ViewModel
{
    public class ÖdemeYapılmayanKoltuklarViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public IEnumerable<Müşteri> ÖdemeYapmayanMüşteriler { get; set; }
    }
}