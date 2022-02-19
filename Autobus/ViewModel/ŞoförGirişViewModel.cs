using Autobus.Model;
using Autobus.Properties;
using Extensions;
using Microsoft.Win32;
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
                if (parameter is Şöförler şöförler && MessageBox.Show("Seçili Şoförü Silmek İstiyor Musun?", Application.Current.MainWindow.Title, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
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
                    Şöför.Resim = openFileDialog.FileName.ResimYükle(Settings.Default.ResimEn, Settings.Default.ResimBoy, Settings.Default.WebpEncode);
                }
            }, parameter => true);
        }

        public Şöför SeçiliŞöför { get; set; }

        public Şöför Şöför { get; set; }

        public ICommand ŞöförEkle { get; }

        public ICommand ŞöförResimYükle { get; }

        public ICommand ŞöförSil { get; }

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