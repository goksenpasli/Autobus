using Autobus.Model;
using Extensions;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Autobus.ViewModel
{
    public class SeferGirişViewModel : INotifyPropertyChanged
    {
        public SeferGirişViewModel()
        {
            Sefer = new Sefer();

            SeferEkle = new RelayCommand<object>(parameter =>
            {
                if (parameter is Otobüs otobüs)
                {
                    if (Sefer.VarışZamanı < DateTime.Now)
                    {
                        MessageBox.Show("Sefer Varış Zamanı Şu Anki Tarihten Daha Önce Devam Edilmez. Gerekli Düzeltmeyi Yapın.", App.Current.MainWindow.Title, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }

                    if (otobüs?.Sefer.Any(z => z.ŞöförId == Sefer.ŞöförId && z.KalkışZamanı >= Sefer.KalkışZamanı && z.KalkışZamanı <= Sefer.VarışZamanı) == true)
                    {
                        MessageBox.Show("Belirtilen Kalkış Saatinde Bu Şoför İçin Başka Bir Sefer Var.", App.Current.MainWindow.Title, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }

                    Sefer sefer = new();
                    sefer.Id = ExtensionMethods.RandomNumber();
                    sefer.BiletTutarı = Math.Round(Sefer.BiletTutarı, 2);
                    sefer.KalkışZamanı = Sefer.KalkışZamanı;
                    sefer.KalkışŞehirId = Sefer.KalkışŞehirId;
                    sefer.AraçId = Sefer.AraçId;
                    sefer.TahminiSüre = Sefer.TahminiSüre;
                    sefer.VarışZamanı = Sefer.VarışZamanı;
                    sefer.VarışŞehirId = Sefer.VarışŞehirId;
                    sefer.ŞöförId = Sefer.ŞöförId;
                    sefer.Renk = ExtensionMethods.RandomColor();
                    otobüs?.Sefer.Add(sefer);
                    MainViewModel.DatabaseSave.Execute(null);

                    ResetSefer();
                }
            }, parameter => Sefer?.VarışZamanı > Sefer?.KalkışZamanı && Sefer?.ŞöförId > 0 && Sefer?.AraçId > 0 && Sefer?.BiletTutarı > 0 && Sefer?.KalkışŞehirId > 0 && Sefer?.VarışŞehirId > 0);

            SeferGüncelle = new RelayCommand<object>(parameter =>
            {
                if (parameter is Otobüs otobüs)
                {
                    if (SeçiliSefer.VarışZamanı < DateTime.Now)
                    {
                        MessageBox.Show("Sefer Varış Zamanı Şu Anki Tarihten Daha Önce Devam Edilmez. Gerekli Düzeltmeyi Yapın.", App.Current.MainWindow.Title, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }

                    if (otobüs?.Sefer.Any(z => z.ŞöförId == SeçiliSefer.ŞöförId && z.KalkışZamanı > SeçiliSefer.KalkışZamanı && z.KalkışZamanı < SeçiliSefer.VarışZamanı) == true)
                    {
                        MessageBox.Show("Belirtilen Kalkış Saatinde Bu Şoför İçin Başka Bir Sefer Var.", App.Current.MainWindow.Title, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }
                    MainViewModel.DatabaseSave.Execute(null);
                }
            }, parameter => SeçiliSefer?.İptal == false && SeçiliSefer?.VarışZamanı > SeçiliSefer?.KalkışZamanı);

            SeferİptalEt = new RelayCommand<object>(parameter =>
            {
                if (MessageBox.Show("Seçili Seferi İptal Etmek İstiyor Musun?", App.Current.MainWindow.Title, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                {
                    SeçiliSefer.İptal = true;
                    MainViewModel.DatabaseSave.Execute(null);
                }
            }, parameter => SeçiliSefer is not null);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Sefer SeçiliSefer { get; set; }

        public Sefer Sefer { get; set; }

        public ICommand SeferEkle { get; }

        public ICommand SeferGüncelle { get; }

        public ICommand SeferİptalEt { get; }

        private void ResetSefer()
        {
            Sefer.BiletTutarı = 0;
            Sefer.KalkışŞehirId = -1;
            Sefer.VarışŞehirId = -1;
            Sefer.AraçId = -1;
            Sefer.TahminiSüre = 0;
        }
    }
}