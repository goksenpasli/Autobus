using Autobus.Model;
using Extensions;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace Autobus.ViewModel
{
    public class YolcuGirişViewModel
    {
        public YolcuGirişViewModel()
        {
            Müşteri = new Müşteri();

            MüşteriEkle = new RelayCommand<object>(parameter =>
            {
                Müşteri müşteri = new();
                müşteri.Id = ExtensionMethods.RandomNumber();
                müşteri.Ad = Müşteri.Ad;
                müşteri.Adres = Müşteri.Adres;
                müşteri.Cinsiyet = Müşteri.Cinsiyet;
                müşteri.Soyad = Müşteri.Soyad;
                müşteri.Telefon = Müşteri.Telefon;
                müşteri.BiletFiyat = SeçiliSefer.BiletTutarı;
                müşteri.BiletÖdendi = Müşteri.BiletÖdendi;
                müşteri.KoltukDolu = true;
                müşteri.KoltukNo = Müşteri.KoltukNo;
                müşteri.Resim = Müşteri.Resim;

                SeçiliSefer.Müşteri.Add(müşteri);
                MainViewModel.DatabaseSave.Execute(null);
                ResetMüşteri();
            }, parameter => SeçiliSefer is not null && SeçiliSefer.VarışZamanı > DateTime.Now && Müşteri.BiletÖdendi && !string.IsNullOrWhiteSpace(Müşteri?.Ad) && !string.IsNullOrWhiteSpace(Müşteri?.Soyad) && !string.IsNullOrWhiteSpace(Müşteri?.Adres) && !string.IsNullOrWhiteSpace(Müşteri?.Telefon) && Müşteri.Cinsiyet != -1);

            MüşteriSil = new RelayCommand<object>(parameter =>
            {
                if (MessageBox.Show("Seçili Müşteriyi Silmek İstiyor Musun?", App.Current.MainWindow.Title, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                {
                    SeçiliSefer.Müşteri.Remove(parameter as Müşteri);
                    MainViewModel.DatabaseSave.Execute(null);
                }
            }, parameter => true);

            MüşteriResimYükle = new RelayCommand<object>(parameter =>
            {
                OpenFileDialog openFileDialog = new() { Multiselect = false, Filter = "Resim Dosyaları (*.jpg;*.jpeg;*.tif;*.tiff;*.png)|*.jpg;*.jpeg;*.tif;*.tiff;*.png" };
                if (openFileDialog.ShowDialog() == true)
                {
                    string filename = Guid.NewGuid() + Path.GetExtension(openFileDialog.FileName);
                    File.Copy(openFileDialog.FileName, $"{Path.GetDirectoryName(MainViewModel.xmldatapath)}\\{filename}");
                    Müşteri.Resim = filename;
                }
            }, parameter => true);
        }

        public Müşteri Müşteri { get; set; }

        public ICommand MüşteriEkle { get; }

        public ICommand MüşteriResimYükle { get; }

        public ICommand MüşteriSil { get; }

        public Araç SeçiliAraç { get; set; }

        public Sefer SeçiliSefer { get; set; }

        private void ResetMüşteri()
        {
            Müşteri.Ad = null;
            Müşteri.Soyad = null;
            Müşteri.Adres = null;
            Müşteri.Telefon = null;
            Müşteri.Resim = null;
            Müşteri.BiletÖdendi = false;
            Müşteri.KoltukNo = -1;
            Müşteri.Cinsiyet = -1;
        }
    }
}