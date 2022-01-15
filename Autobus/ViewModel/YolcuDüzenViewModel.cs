using Autobus.Model;
using Extensions;
using System.Windows;
using System.Windows.Input;

namespace Autobus.ViewModel
{
    public class YolcuDüzenViewModel
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
                }
            }, parameter => true);
        }

        public ICommand MüşteriBorçTahsilEt { get; }

        public ICommand MüşteriEkleEkranı { get; }
    }
}