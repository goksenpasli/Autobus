using Autobus.Model;
using Autobus.View;
using System.Collections.Generic;
using System.ComponentModel;

namespace Autobus.ViewModel
{
    public class TümSeferlerViewModel : INotifyPropertyChanged
    {
        public TümSeferlerViewModel()
        {
            PropertyChanged += TümSeferlerViewModel_PropertyChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string MüşteriAdArama { get; set; }

        public IEnumerable<Müşteri> Müşteriler { get; set; }

        public string MüşteriSoyadArama { get; set; }

        private void TümSeferlerViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is "MüşteriAdArama")
            {
                TümSeferlerView.cvs.Filter += (s, e) => e.Accepted &= (e.Item as Müşteri)?.Ad.Contains(MüşteriAdArama) == true;
            }
            if (e.PropertyName is "MüşteriSoyadArama")
            {
                TümSeferlerView.cvs.Filter += (s, e) => e.Accepted &= (e.Item as Müşteri)?.Ad.Contains(MüşteriSoyadArama) == true;
            }
        }
    }
}