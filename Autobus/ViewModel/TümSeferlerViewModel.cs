using Autobus.Model;
using Autobus.View;
using Extensions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Autobus.ViewModel
{
    public class TümSeferlerViewModel : INotifyPropertyChanged
    {
        public TümSeferlerViewModel()
        {
            BiletYazdır = new RelayCommand<object>(parameter => YolcuGirişViewModel.PrintTicket(parameter), parameter => true);
            MüşteriSil = new RelayCommand<object>(parameter =>
            {
                if (MessageBox.Show("Seçili Müşteriyi Silmek İstiyor Musun?", App.Current.MainWindow.Title, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                {
                    foreach (Sefer item in Seferler)
                    {
                        foreach (Müşteri müşteri in Müşteriler.ToList())
                        {
                            if (müşteri.Id == (parameter as Müşteri)?.Id)
                            {
                                item.Müşteri.Remove(müşteri);
                            }
                        }
                    }
                    MainViewModel.DatabaseSave.Execute(null);
                    OnPropertyChanged(nameof(Seferler));
                }
            }, parameter => true);
            PropertyChanged += TümSeferlerViewModel_PropertyChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand BiletYazdır { get; }

        public int KalkışŞehirAramaId { get; set; }

        public string MüşteriAdArama { get; set; }

        public IEnumerable<Müşteri> Müşteriler { get; set; }

        public ICommand MüşteriSil { get; }

        public string MüşteriSoyadArama { get; set; }

        public ObservableCollection<Sefer> Seferler { get; set; }

        public int VarışŞehirAramaId { get; set; }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

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
                if (KalkışŞehirAramaId == 0)
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
            if (e.PropertyName is "Seferler")
            {
                Müşteriler = Seferler?.SelectMany(z => z.Müşteri);
            }
        }
    }
}