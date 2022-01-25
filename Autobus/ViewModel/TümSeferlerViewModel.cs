using Autobus.Model;
using Autobus.View;
using Extensions;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

namespace Autobus.ViewModel
{
    public class TümSeferlerViewModel : INotifyPropertyChanged
    {
        public TümSeferlerViewModel()
        {
            BiletYazdır = new RelayCommand<object>(parameter => YolcuGirişViewModel.PrintTicket(parameter), parameter => true);

            PropertyChanged += TümSeferlerViewModel_PropertyChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand BiletYazdır { get; }

        public int KalkışŞehirAramaId { get; set; }

        public string MüşteriAdArama { get; set; }

        public IEnumerable<Müşteri> Müşteriler { get; set; }

        public string MüşteriSoyadArama { get; set; }

        public int VarışŞehirAramaId { get; set; }

        public override string ToString()
        {
            return "TÜM SEFERLER";
        }

        private void TümSeferlerViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is "MüşteriAdArama")
            {
                TümSeferlerView.cvs.Filter += (s, e) => e.Accepted &= (e.Item as Müşteri)?.Ad.Contains(MüşteriAdArama) == true;
            }
            if (e.PropertyName is "KalkışŞehirAramaId")
            {
                if (KalkışŞehirAramaId==0)
                {
                    TümSeferlerView.cvs.Filter += (s, e) => e.Accepted = true;
                    return;
                }
                TümSeferlerView.cvs.Filter += (s, e) => e.Accepted &= (e.Item as Müşteri)?.SeçiliSefer.KalkışŞehirId == KalkışŞehirAramaId;
            }
            if (e.PropertyName is "VarışŞehirAramaId")
            {
                if (VarışŞehirAramaId == 0)
                {
                    TümSeferlerView.cvs.Filter += (s, e) => e.Accepted = true;
                    return;
                }
                TümSeferlerView.cvs.Filter += (s, e) => e.Accepted &= (e.Item as Müşteri)?.SeçiliSefer.VarışŞehirId == VarışŞehirAramaId;
            }
            if (e.PropertyName is "MüşteriSoyadArama")
            {
                TümSeferlerView.cvs.Filter += (s, e) => e.Accepted &= (e.Item as Müşteri)?.Ad.Contains(MüşteriSoyadArama) == true;
            }
        }
    }
}