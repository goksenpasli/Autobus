﻿using Autobus.Model;
using Extensions;
using System.Windows.Input;

namespace Autobus.ViewModel
{
    public class AraçGirişViewModel
    {
        public AraçGirişViewModel()
        {
            Araç = new Araç();
            Marka = new Marka();

            AraçEkle = new RelayCommand<object>(parameter =>
            {
                Araç araç = new();
                araç.Id = ExtensionMethods.RandomNumber();
                araç.Aktif = Araç.Aktif;
                araç.BölmeSayısı = Araç.BölmeSayısı;
                araç.KoltukSayısı = Araç.KoltukSayısı;
                araç.Plaka = Araç.Plaka;
                araç.MarkaId = Araç.MarkaId;
                araç.BoyKoltukSayısı = Araç.BoyKoltukSayısı;
                araç.Resim = Araç.Resim;

                (parameter as Araçlar)?.Araç.Add(araç);
                MainViewModel.DatabaseSave.Execute(null);
            }, parameter => Araç?.MarkaId != 0 && !string.IsNullOrWhiteSpace(Araç?.Plaka));

            MarkaEkle = new RelayCommand<object>(parameter =>
            {
                Marka marka = new();
                marka.Id = ExtensionMethods.RandomNumber();
                marka.Açıklama = Marka.Açıklama;

                (parameter as Markalar)?.Marka.Add(marka);
                MainViewModel.DatabaseSave.Execute(null);
            }, parameter => !string.IsNullOrWhiteSpace(Marka?.Açıklama));
        }

        public Araç Araç { get; set; }

        public ICommand AraçEkle { get; }

        public Marka Marka { get; set; }

        public ICommand MarkaEkle { get; }
    }
}