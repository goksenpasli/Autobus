using Autobus.Model;
using Autobus.Properties;
using Extensions;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Autobus.ViewModel
{
    public class YolcuGirişViewModel : INotifyPropertyChanged
    {
        public YolcuGirişViewModel()
        {
            Müşteri = new Müşteri();
            Ürün = new Ürün();

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

            ÜrünEkle = new RelayCommand<object>(parameter =>
            {
                Ürün ürün = new();
                ürün.Id = ExtensionMethods.RandomNumber();
                ürün.ÜrünFiyat = Ürün.ÜrünFiyat;
                ürün.ÜrünAçıklama = Ürün.ÜrünAçıklama;
                (parameter as Ürünler)?.Ürün?.Add(ürün);
                MainViewModel.DatabaseSave.Execute(null);
            }, parameter => !string.IsNullOrWhiteSpace(Ürün.ÜrünAçıklama));

            MüşteriSiparişEkle = new RelayCommand<object>(parameter =>
             {
                 if (MessageBox.Show("Seçili Ürünü Müşteriye Satmak İstiyor Musun?", App.Current.MainWindow.Title, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                 {
                     Sipariş sipariş = new();
                     sipariş.Id = ExtensionMethods.RandomNumber();
                     sipariş.ÜrünId = SeçiliÜrün.Id;
                     SeçiliMüşteri.Sipariş.Add(sipariş);
                     MainViewModel.DatabaseSave.Execute(null);
                 }
             }, parameter => SeçiliMüşteri is not null && SeçiliÜrün is not null);

            MüşteriResimYükle = new RelayCommand<object>(parameter =>
            {
                OpenFileDialog openFileDialog = new() { Multiselect = false, Filter = "Resim Dosyaları (*.jpg;*.jpeg;*.tif;*.tiff;*.png)|*.jpg;*.jpeg;*.tif;*.tiff;*.png" };
                if (openFileDialog.ShowDialog() == true)
                {
                    string filename = Guid.NewGuid() + Path.GetExtension(openFileDialog.FileName);
                    image = new BitmapImage(new Uri(openFileDialog.FileName));
                    File.WriteAllBytes($"{Path.GetDirectoryName(MainViewModel.xmldatapath)}\\{filename}", image.Resize(Settings.Default.ResimEn, Settings.Default.ResimBoy).ToTiffJpegByteArray(Extensions.ExtensionMethods.Format.Jpg));
                    Müşteri.Resim = filename;
                }
            }, parameter => true);

            BiletYazdır = new RelayCommand<object>(parameter =>
            {
                PrintDialog printDlg = new();
                if (printDlg.ShowDialog() == true)
                {
                    printDlg.PrintVisual(parameter as Visual, "Bilet Yazdır.");
                }
            }, parameter => SeçiliSefer is not null && SeçiliMüşteri is not null);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Müşteri Müşteri { get; set; }

        public ICommand MüşteriEkle { get; }

        public ICommand MüşteriResimYükle { get; }

        public ICommand MüşteriSil { get; }

        public ICommand BiletYazdır { get; }

        public ICommand MüşteriSiparişEkle { get; }

        public Araç SeçiliAraç { get; set; }

        public Müşteri SeçiliMüşteri { get; set; }

        public Sefer SeçiliSefer { get; set; }

        public Ürün SeçiliÜrün { get; set; }

        public Ürün Ürün { get; set; }

        public ICommand ÜrünEkle { get; }

        private BitmapImage image;

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
            //SeçiliSefer = null;
        }
    }
}