using Autobus.Model;
using Autobus.Properties;
using Extensions;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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
                müşteri.KoltukNo = Müşteri.KoltukNo;
                müşteri.Resim = Müşteri.Resim;
                müşteri.SeferId = SeçiliSefer.Id;
                müşteri.KoltukDolu = true;
                if (Müşteri.BiletÖdendi)
                {
                    müşteri.BiletÖdendi = true;
                }
                else if (MessageBox.Show("Bilet Ödenmedi Devam Ederseniz Giriş Yapılır Ancak Yolculuktan Önce Biletin Ödenmesi Gerekir.\nDevam Etmek İstiyor Musun?", App.Current.MainWindow.Title, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                {
                    müşteri.BiletÖdendi = false;
                }
                else
                {
                    return;
                }

                SeçiliSefer.Müşteri.Add(müşteri);
                MainViewModel.DatabaseSave.Execute(null);
                ResetMüşteri();
            }, parameter => SeçiliSefer is not null && Müşteri.KoltukNo > 0 && SeçiliSefer.VarışZamanı > DateTime.Now && !string.IsNullOrWhiteSpace(Müşteri.Ad) && !string.IsNullOrWhiteSpace(Müşteri.Soyad) && !string.IsNullOrWhiteSpace(Müşteri.Adres) && Müşteri.Telefon?.Length == 10 && Müşteri.Cinsiyet > -1);

            ÜrünEkle = new RelayCommand<object>(parameter =>
            {
                Ürün ürün = new();
                ürün.Id = ExtensionMethods.RandomNumber();
                ürün.ÜrünFiyat = Math.Round(Ürün.ÜrünFiyat, 2);
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
             }, parameter => SeçiliMüşteri is not null && SeçiliÜrün is not null && SeçiliSefer?.İptal == false);

            MüşteriResimYükle = new RelayCommand<object>(parameter =>
            {
                OpenFileDialog openFileDialog = new() { Multiselect = false, Filter = "Resim Dosyaları (*.jpg;*.jpeg;*.tif;*.tiff;*.png)|*.jpg;*.jpeg;*.tif;*.tiff;*.png" };
                if (openFileDialog.ShowDialog() == true)
                {
                    Müşteri.Resim = openFileDialog.FileName.ResimYükle(Settings.Default.ResimEn, Settings.Default.ResimBoy, Settings.Default.WebpEncode);
                }
            }, parameter => true);

            BiletYazdır = new RelayCommand<object>(parameter => PrintTicket(parameter), parameter => SeçiliSefer is not null && SeçiliMüşteri is not null);

            YolcuAktar = new RelayCommand<object>(parameter =>
            {
                if (parameter is Müşteri müşteri)
                {
                    Müşteri.Ad = müşteri.Ad;
                    Müşteri.Soyad = müşteri.Soyad;
                    Müşteri.Adres = müşteri.Adres;
                    Müşteri.Telefon = müşteri.Telefon;
                    Müşteri.Cinsiyet = müşteri.Cinsiyet;
                    Müşteri.Resim = müşteri.Resim;
                }
            }, parameter => parameter is Müşteri müşteri);

            YolcuAramaEkranı = new RelayCommand<object>(parameter =>
            {
                if (parameter is Otobüs otobüs)
                {
                    YolcuAramaMetni = string.Empty;
                    AramaYolcuListesi = otobüs?.Sefer?.SelectMany(z => z.Müşteri);
                }
            }, parameter => true);

            PropertyChanged += YolcuGirişViewModel_PropertyChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public IEnumerable<Müşteri> AramaYolcuListesi { get; set; }

        public ICommand BiletYazdır { get; }

        public Müşteri Müşteri { get; set; }

        public ICommand MüşteriEkle { get; }

        public ICommand MüşteriResimYükle { get; }

        public ICommand MüşteriSiparişEkle { get; }

        public Araç SeçiliAraç { get; set; }

        public Müşteri SeçiliMüşteri { get; set; }

        public Sefer SeçiliSefer { get; set; }

        public Ürün SeçiliÜrün { get; set; }

        public Ürün Ürün { get; set; }

        public ICommand ÜrünEkle { get; }

        public ICommand YolcuAktar { get; }

        public ICommand YolcuAramaEkranı { get; }

        public string YolcuAramaMetni { get; set; }

        public static void PrintTicket(object parameter)
        {
            PrintDialog printDlg = new();
            if (printDlg.ShowDialog() == true)
            {
                printDlg.PrintVisual(parameter as Visual, "Bilet Yazdır.");
            }
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return "YOLCU GİRİŞ";
        }

        private void ResetMüşteri()
        {
            Müşteri.Ad = null;
            Müşteri.Soyad = null;
            Müşteri.Adres = null;
            Müşteri.Telefon = null;
            Müşteri.Resim = null;
            Müşteri.BiletÖdendi = false;
            Müşteri.Cinsiyet = -1;
            OnPropertyChanged(nameof(SeçiliSefer));
        }

        private void YolcuGirişViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is "SeçiliSefer")
            {
                Müşteri.KoltukNo = 0;
            }
            if (e.PropertyName is "YolcuAramaMetni")
            {
                AramaYolcuListesi = AramaYolcuListesi?.Where(z => z.Ad.Contains(YolcuAramaMetni) || z.Soyad.Contains(YolcuAramaMetni));
            }
        }
    }
}