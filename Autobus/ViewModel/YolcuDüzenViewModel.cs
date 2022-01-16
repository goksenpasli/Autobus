using Autobus.Model;
using Extensions;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Autobus.ViewModel
{
    public class YolcuDüzenViewModel : INotifyPropertyChanged
    {
        public YolcuDüzenViewModel()
        {
            MüşteriEkleEkranı = new RelayCommand<object>(parameter =>
            {
                if (parameter is object[] data && data[0] is MainViewModel mainViewModel && data[1] is int koltukno && data[2] is Sefer sefer && data[3] is Araç araç)
                {
                    mainViewModel.YolcuGirişViewModel.SeçiliSefer = sefer;
                    mainViewModel.YolcuGirişViewModel.SeçiliAraç = araç;
                    mainViewModel.YolcuGirişViewModel.Müşteri.KoltukNo = koltukno;
                    mainViewModel.CurrentView = mainViewModel.YolcuGirişViewModel;
                }
            }, parameter => true);

            MüşteriBorçTahsilEt = new RelayCommand<object>(parameter =>
            {
                if (parameter is Müşteri müşteri && MessageBox.Show($"{müşteri.BiletFiyat:C} Bilet Tahsilatını Onaylıyor musun?", App.Current.MainWindow.Title, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                {
                    müşteri.BiletÖdendi = true;
                    müşteri.KoltukDolu = true;
                    MainViewModel.DatabaseSave.Execute(null);
                    SayıAyarla();
                }
            }, parameter => true);
            PropertyChanged += YolcuDüzenViewModel_PropertyChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public int ErkekSayısı { get; set; }

        public int KadınSayısı { get; set; }

        public ICommand MüşteriBorçTahsilEt { get; }

        public ICommand MüşteriEkleEkranı { get; }

        public int ÖdemeYapılmamışSayısı { get; set; }

        public Sefer SeçiliSefer { get; set; }

        private void SayıAyarla()
        {
            ErkekSayısı = SeçiliSefer?.Müşteri.Count(z => z.Cinsiyet == 0) ?? 0;
            KadınSayısı = SeçiliSefer?.Müşteri.Count(z => z.Cinsiyet == 1) ?? 0;
            ÖdemeYapılmamışSayısı = SeçiliSefer?.Müşteri.Count(z => !z.BiletÖdendi) ?? 0;
        }

        private void YolcuDüzenViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is "SeçiliSefer")
            {
                SayıAyarla();
            }
        }
    }
}