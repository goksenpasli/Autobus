using Autobus.Model;
using Extensions;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace Autobus.ViewModel
{
    public class ŞoförGirişViewModel
    {
        public ŞoförGirişViewModel()
        {
            Şöför = new Şöför();

            ŞöförEkle = new RelayCommand<object>(parameter =>
            {
                Şöför şöför = new();
                şöför.Id = ExtensionMethods.RandomNumber();
                şöför.Ad = Şöför.Ad;
                şöför.Adres = Şöför.Adres;
                şöför.Soyad = Şöför.Soyad;
                şöför.Telefon = Şöför.Telefon;
                şöför.Resim = Şöför.Resim;
                şöför.Etkin = true;

                (parameter as Otobüs)?.Şöförler.Şöför.Add(şöför);
                MainViewModel.DatabaseSave.Execute(null);
            }, parameter => !string.IsNullOrWhiteSpace(Şöför?.Ad) && !string.IsNullOrWhiteSpace(Şöför?.Soyad));

            ŞöförSil = new RelayCommand<object>(parameter =>
            {
                if (MessageBox.Show("Seçili Şoförü Silmek İstiyor Musun?", App.Current.MainWindow.Title, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                {
                    (parameter as Otobüs)?.Şöförler.Şöför?.Remove(SeçiliŞöför);
                    MainViewModel.DatabaseSave.Execute(null);
                }
            }, parameter => SeçiliŞöför is not null);

            ŞöförResimYükle = new RelayCommand<object>(parameter =>
            {
                OpenFileDialog openFileDialog = new() { Multiselect = false, Filter = "Resim Dosyaları (*.jpg;*.jpeg;*.tif;*.tiff;*.png)|*.jpg;*.jpeg;*.tif;*.tiff;*.png" };
                if (openFileDialog.ShowDialog() == true)
                {
                    string filename = Guid.NewGuid() + Path.GetExtension(openFileDialog.FileName);
                    File.Copy(openFileDialog.FileName, $"{Path.GetDirectoryName(MainViewModel.xmldatapath)}\\{filename}");
                    Şöför.Resim = filename;
                }
            }, parameter => true);
        }

        public Şöför SeçiliŞöför { get; set; }

        public Şöför Şöför { get; set; }

        public ICommand ŞöförEkle { get; }

        public ICommand ŞöförResimYükle { get; }

        public ICommand ŞöförSil { get; }
    }
}