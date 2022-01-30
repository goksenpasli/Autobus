using Autobus.Model;
using Autobus.Properties;
using Extensions;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Autobus.ViewModel
{
    public class ŞoförGirişViewModel
    {
        public ŞoförGirişViewModel()
        {
            Şöför = new Şöför();

            ŞöförEkle = new RelayCommand<object>(parameter =>
            {
                if (parameter is Şöförler şöförler)
                {
                    Şöför şöför = new();
                    şöför.Id = ExtensionMethods.RandomNumber();
                    şöför.Ad = Şöför.Ad;
                    şöför.Adres = Şöför.Adres;
                    şöför.Soyad = Şöför.Soyad;
                    şöför.Telefon = Şöför.Telefon;
                    şöför.Resim = Şöför.Resim;
                    şöför.Etkin = true;

                    şöförler?.Şöför.Add(şöför);
                    MainViewModel.DatabaseSave.Execute(null);
                    ResetŞöför();
                }
            }, parameter => !string.IsNullOrWhiteSpace(Şöför?.Ad) && !string.IsNullOrWhiteSpace(Şöför?.Soyad));

            ŞöförSil = new RelayCommand<object>(parameter =>
            {
                if (parameter is Şöförler şöförler && MessageBox.Show("Seçili Şoförü Silmek İstiyor Musun?", App.Current.MainWindow.Title, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                {
                    şöförler?.Şöför?.Remove(SeçiliŞöför);
                    MainViewModel.DatabaseSave.Execute(null);
                }
            }, parameter => SeçiliŞöför?.Etkin == true);

            ŞöförResimYükle = new RelayCommand<object>(parameter =>
            {
                OpenFileDialog openFileDialog = new() { Multiselect = false, Filter = "Resim Dosyaları (*.jpg;*.jpeg;*.tif;*.tiff;*.png)|*.jpg;*.jpeg;*.tif;*.tiff;*.png" };
                if (openFileDialog.ShowDialog() == true)
                {
                    string filename = Guid.NewGuid() + Path.GetExtension(openFileDialog.FileName);
                    image = new BitmapImage(new Uri(openFileDialog.FileName));
                    File.WriteAllBytes($"{Path.GetDirectoryName(MainViewModel.xmldatapath)}\\{filename}", image.Resize(Settings.Default.ResimEn, Settings.Default.ResimBoy).ToTiffJpegByteArray(Extensions.ExtensionMethods.Format.Jpg));
                    Şöför.Resim = filename;
                }
            }, parameter => true);
        }

        public Şöför SeçiliŞöför { get; set; }

        public Şöför Şöför { get; set; }

        public ICommand ŞöförEkle { get; }

        public ICommand ŞöförResimYükle { get; }

        public ICommand ŞöförSil { get; }

        private BitmapImage image;

        private void ResetŞöför()
        {
            Şöför.Ad = null;
            Şöför.Adres = null;
            Şöför.Soyad = null;
            Şöför.Telefon = null;
            Şöför.Resim = null;
            Şöför.Etkin = true;
        }
    }
}