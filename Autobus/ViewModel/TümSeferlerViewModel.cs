﻿using Autobus.Model;
using Autobus.View;
using Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Autobus.ViewModel
{
    public class TümSeferlerViewModel : INotifyPropertyChanged
    {
        public TümSeferlerViewModel()
        {
            BiletYazdır = new RelayCommand<object>(parameter => YolcuGirişViewModel.PrintTicket(parameter), parameter => true);
            MüşteriSil = new RelayCommand<object>(parameter =>
            {
                if (MessageBox.Show($"{SeçiliMüşteri.Ad} {SeçiliMüşteri.Soyad} Adlı Müşteriyi Silmek İstiyor Musun?", App.Current.MainWindow.Title, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                {
                    SeçiliMüşteri?.SeçiliSefer?.Müşteri?.Remove(SeçiliMüşteri);
                    MainViewModel.DatabaseSave.Execute(null);
                    OnPropertyChanged(nameof(Otobüs));
                }
            }, parameter => SeçiliMüşteri is not null);

            MüşteriTaşı = new RelayCommand<object>(parameter =>
            {
                if (MessageBox.Show($"{SeçiliMüşteri.Ad} {SeçiliMüşteri.Soyad} Adlı Müşteriyi {(int)parameter} Nolu Koltuğa Taşımak İstiyor Musun?", App.Current.MainWindow.Title, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                {
                    SeçiliMüşteri.KoltukNo = (int)parameter;
                    MainViewModel.DatabaseSave.Execute(null);
                    OnPropertyChanged(nameof(SeçiliMüşteri));
                }
            }, parameter => true);
            PropertyChanged += TümSeferlerViewModel_PropertyChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool? AramaBiletÖdendi { get; set; }

        public ICommand BiletYazdır { get; }

        public IEnumerable<int> BoşKalanKoltuklar { get; set; }

        public int KalkışŞehirAramaId { get; set; }

        public DateTime? KalkışTarihArama { get; set; }

        public string MüşteriAdArama { get; set; }

        public IEnumerable<Müşteri> Müşteriler { get; set; }

        public ICommand MüşteriSil { get; }

        public string MüşteriSoyadArama { get; set; }

        public ICommand MüşteriTaşı { get; }

        public Otobüs Otobüs { get; set; }

        public Araç SeçiliAraç { get; set; }

        public Müşteri SeçiliMüşteri { get; set; }

        public ObservableCollection<Sefer> Seferler { get; set; }

        public string TelefonArama { get; set; }

        public int VarışŞehirAramaId { get; set; }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return "TÜM SEFERLER";
        }

        private void TümSeferlerViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is "MüşteriAdArama")
            {
                TümSeferlerView.cvs.Filter += (s, e) => e.Accepted &= (e.Item as Müşteri)?.Ad.Contains(MüşteriAdArama) == true;
            }
            if (e.PropertyName is "TelefonArama")
            {
                TümSeferlerView.cvs.Filter += (s, e) => e.Accepted &= (e.Item as Müşteri)?.Telefon.Contains(TelefonArama) == true;
            }
            if (e.PropertyName is "KalkışŞehirAramaId")
            {
                if (KalkışŞehirAramaId == 0)
                {
                    TümSeferlerView.cvs.Filter += (s, e) => e.Accepted = true;
                    return;
                }
                TümSeferlerView.cvs.Filter += (s, e) => e.Accepted &= (e.Item as Müşteri)?.SeçiliSefer.KalkışŞehirId == KalkışŞehirAramaId;
            }
            if (e.PropertyName is "KalkışTarihArama")
            {
                if (KalkışTarihArama == null)
                {
                    TümSeferlerView.cvs.Filter += (s, e) => e.Accepted = true;
                    return;
                }
                TümSeferlerView.cvs.Filter += (s, e) => e.Accepted &= (e.Item as Müşteri)?.SeçiliSefer.KalkışZamanı == KalkışTarihArama;
            }
            if (e.PropertyName is "AramaBiletÖdendi")
            {
                if (AramaBiletÖdendi == null)
                {
                    TümSeferlerView.cvs.Filter += (s, e) => e.Accepted = true;
                    return;
                }
                TümSeferlerView.cvs.Filter += (s, e) => e.Accepted &= (e.Item as Müşteri)?.BiletÖdendi == AramaBiletÖdendi;
            }
            if (e.PropertyName is "VarışŞehirAramaId")
            {
                if (VarışŞehirAramaId == 0)
                {
                    TümSeferlerView.cvs.Filter += (s, e) => e.Accepted = true;
                    return;
                }
                TümSeferlerView.cvs.Filter += (s, e) => e.Accepted &= (e.Item as Müşteri)?.SeçiliSefer.VarışŞehirId == VarışŞehirAramaId;
            }
            if (e.PropertyName is "MüşteriSoyadArama")
            {
                TümSeferlerView.cvs.Filter += (s, e) => e.Accepted &= (e.Item as Müşteri)?.Ad.Contains(MüşteriSoyadArama) == true;
            }
            if (e.PropertyName is "Otobüs")
            {
                Müşteriler = Otobüs.Sefer?.SelectMany(z => z.Müşteri);
            }
            if (e.PropertyName is "SeçiliMüşteri")
            {
                SeçiliAraç = Otobüs.Araçlar.Araç.FirstOrDefault(z => z.Id == SeçiliMüşteri?.SeçiliSefer?.AraçId);
                BoşKalanKoltuklar = SeçiliAraç?.KoltukÖnizlemeListe.Except(SeçiliAraç?.GizlenenKoltuklar)?.Except(Müşteriler?.Where(z => z.SeferId == SeçiliMüşteri?.SeferId && z.KoltukDolu).Select(z => z.KoltukNo));
            }
        }
    }
}