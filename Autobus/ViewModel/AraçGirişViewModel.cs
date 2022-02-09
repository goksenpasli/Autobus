using Autobus.Model;
using Extensions;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Autobus.ViewModel
{
    public class AraçGirişViewModel
    {
        public AraçGirişViewModel()
        {
            Araç = new Araç();
            Marka = new Marka();
            Araç.GizlenenKoltuklar = new();
            AraçEkle = new RelayCommand<object>(parameter =>
            {
                Araç araç = new();
                araç.Id = ExtensionMethods.RandomNumber();
                araç.Aktif = Araç.Aktif;
                araç.BölmeSayısı = Araç.BölmeSayısı;
                araç.KoltukSayısı = Araç.KoltukSayısı;
                araç.Plaka = Regex.Replace(Araç.Plaka, @"\s+", " ");
                araç.MarkaId = Araç.MarkaId;
                araç.BoyKoltukSayısı = Araç.BoyKoltukSayısı;
                araç.Resim = Araç.Resim;
                Araç.GizlenenKoltuklar.ToList().ForEach(z => araç.GizlenenKoltuklar?.Add(z));
                (parameter as Araçlar)?.Araç.Add(araç);
                MainViewModel.DatabaseSave.Execute(null);

                ResetAraç();
            }, parameter => Araç.KoltukSayısı > 0 && Araç.KoltukSayısı >= Araç.BölmeSayısı && Araç?.MarkaId != -1 && Araç?.KoltukSayısı % Araç?.BölmeSayısı == 0 && !string.IsNullOrWhiteSpace(Araç?.Plaka));

            MarkaEkle = new RelayCommand<object>(parameter =>
            {
                if (parameter is Markalar markalar)
                {
                    Marka marka = new();
                    marka.Id = ExtensionMethods.RandomNumber();
                    marka.Açıklama = Marka.Açıklama;
                    if (markalar?.Marka.Any(z => z.Açıklama == marka.Açıklama) == false)
                    {
                        markalar?.Marka.Add(marka);
                        MainViewModel.DatabaseSave.Execute(null);
                    }
                }
            }, parameter => !string.IsNullOrWhiteSpace(Marka?.Açıklama));

            AraçResimYükle = new RelayCommand<object>(parameter =>
            {
                OpenFileDialog openFileDialog = new() { Multiselect = false, Filter = "Resim Dosyaları (*.jpg;*.jpeg;*.tif;*.tiff;*.png)|*.jpg;*.jpeg;*.tif;*.tiff;*.png" };
                if (openFileDialog.ShowDialog() == true)
                {
                    string filename = Guid.NewGuid() + Path.GetExtension(openFileDialog.FileName);
                    image = new BitmapImage(new Uri(openFileDialog.FileName));
                    File.WriteAllBytes($"{Path.GetDirectoryName(MainViewModel.xmldatapath)}\\{filename}", image.Resize(ResimEn, ResimBoy).ToTiffJpegByteArray(Extensions.ExtensionMethods.Format.Jpg));
                    Araç.Resim = filename;
                }
            }, parameter => true);

            GizlenenKoltuklaraEkle = new RelayCommand<object>(parameter =>
            {
                if (parameter is int koltukno && Araç.GizlenenKoltuklar?.Contains(koltukno) == false)
                {
                    Araç.GizlenenKoltuklar?.Add(koltukno);
                }
            }, parameter => Araç.GizlenenKoltuklar?.Count + 1 < Araç.KoltukSayısı);

            GizlenenKoltuklardanSil = new RelayCommand<object>(parameter =>
            {
                if (parameter is int koltukno)
                {
                    Araç.GizlenenKoltuklar?.Remove(koltukno);
                }
            }, parameter => true);
        }

        public Araç Araç { get; set; }

        public ICommand AraçEkle { get; }

        public ICommand AraçResimYükle { get; }

        public ICommand GizlenenKoltuklaraEkle { get; }

        public ICommand GizlenenKoltuklardanSil { get; }

        public Marka Marka { get; set; }

        public ICommand MarkaEkle { get; }

        public double ResimBoy { get; set; }

        public double ResimEn { get; set; }

        public override string ToString()
        {
            return "ARAÇ GİRİŞ";
        }

        private BitmapImage image;

        private void ResetAraç()
        {
            Araç.KoltukSayısı = 0;
            Araç.BölmeSayısı = 1;
            Araç.MarkaId = -1;
            Araç.Plaka = null;
            Araç.Resim = null;
            Araç.Aktif = true;
            Araç.GizlenenKoltuklar?.Clear();
        }
    }
}