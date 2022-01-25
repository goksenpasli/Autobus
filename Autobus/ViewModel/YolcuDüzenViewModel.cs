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
                if (parameter is Müşteri müşteri && MessageBox.Show($"{müşteri.Ad} {müşteri.Soyad} adlı kişiden {müşteri.BiletFiyat:C} bilet tahsilatını onaylıyor musun?", App.Current.MainWindow.Title, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                {
                    müşteri.BiletÖdendi = true;
                    müşteri.KoltukDolu = true;
                    MainViewModel.DatabaseSave.Execute(null);
                    SayılarıGüncelle(SeçiliSefer, SeçiliAraç);
                }
            }, parameter => parameter is Müşteri müşteri && !müşteri.BiletÖdendi);
            PropertyChanged += YolcuDüzenViewModel_PropertyChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public double DolulukOranı { get; private set; }

        public int ErkekSayısı { get; private set; }

        public int KadınSayısı { get; private set; }

        public ICommand MüşteriBorçTahsilEt { get; }

        public ICommand MüşteriEkleEkranı { get; }

        public int ÖdemeYapılmamışSayısı { get; private set; }

        public Araç SeçiliAraç { get; set; }

        public Sefer SeçiliSefer { get; set; }

        public double ToplamGelir { get; private set; }

        private void SayılarıGüncelle(Sefer sefer, Araç araç)
        {
            ErkekSayısı = sefer?.Müşteri.Count(z => z.Cinsiyet == 0) ?? 0;
            KadınSayısı = sefer?.Müşteri.Count(z => z.Cinsiyet == 1) ?? 0;
            ÖdemeYapılmamışSayısı = sefer?.Müşteri.Count(z => !z.BiletÖdendi) ?? 0;
            ToplamGelir = sefer?.Müşteri.Where(z => z.BiletÖdendi).Sum(z => z.BiletFiyat) ?? 0;
            DolulukOranı = araç != null ? (double)((ErkekSayısı + KadınSayısı) / (double)(araç.KoltukSayısı - araç.GizlenenKoltuklar.Count)) : 0;
        }

        private void YolcuDüzenViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is "SeçiliAraç" or "SeçiliSefer")
            {
                SayılarıGüncelle(SeçiliSefer, SeçiliAraç);
            }
        }
    }
}