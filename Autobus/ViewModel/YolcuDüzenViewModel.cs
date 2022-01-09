using Autobus.Model;
using Extensions;
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
                    YolcuGirişViewModel yolcuGirişViewModel = mainViewModel.YolcuGirişViewModel;
                    yolcuGirişViewModel.SeçiliSefer = sefer;
                    yolcuGirişViewModel.SeçiliAraç = araç;
                    yolcuGirişViewModel.Müşteri.KoltukNo = koltukno;
                    mainViewModel.CurrentView = yolcuGirişViewModel;
                }
            }, parameter => true);
        }

        public ICommand MüşteriEkleEkranı { get; }
    }
}