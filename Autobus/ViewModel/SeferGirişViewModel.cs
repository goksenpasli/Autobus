using Autobus.Model;
using Extensions;
using System;
using System.Windows;
using System.Windows.Input;

namespace Autobus.ViewModel
{
    public class SeferGirişViewModel
    {
        public SeferGirişViewModel()
        {
            Sefer = new Sefer();

            SeferEkle = new RelayCommand<object>(parameter =>
            {
                if (Sefer.VarışZamanı < DateTime.Now)
                {
                    MessageBox.Show("Sefer Varış Zamanı Şu Anki Tarihten Daha Önce Devam Edilmez. Gerekli Düzeltmeyi Yapın.", App.Current.MainWindow.Title, MessageBoxButton.OK, MessageBoxImage.Exclamation);
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

                (parameter as Otobüs)?.Sefer.Add(sefer);
                MainViewModel.DatabaseSave.Execute(null);

                ResetSefer();
            }, parameter => Sefer?.ŞöförId != 0 && Sefer?.AraçId != 0 && Sefer?.TahminiSüre != 0 && Sefer?.BiletTutarı != 0 && Sefer?.KalkışŞehirId != 0 && Sefer?.VarışŞehirId != 0);
        }

        public Sefer Sefer { get; set; }

        public ICommand SeferEkle { get; }

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