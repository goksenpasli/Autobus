using Autobus.Model;
using Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Input;
using static Extensions.GraphControl;

namespace Autobus.ViewModel
{
    public class ÖdemeYapılmayanKoltuklarViewModel : INotifyPropertyChanged
    {
        public ÖdemeYapılmayanKoltuklarViewModel()
        {
            CsvDosyasınaYaz = new RelayCommand<object>(parameter =>
            {
                string dosyaismi = Path.GetTempPath() + Guid.NewGuid() + ".csv";
                string seperator = new CultureInfo(CultureInfo.CurrentCulture.Name).TextInfo.ListSeparator;
                File.AppendAllText(dosyaismi, $"AY{seperator}TOPLAM TUTAR{seperator}SİPARİŞ TUTAR\n", Encoding.UTF8);
                foreach (Tahsilat tahsilat in Tahsilatlar)
                {
                    File.AppendAllText(dosyaismi, $"{tahsilat.Tarih}{seperator}{tahsilat.ToplamTutar}{seperator}{tahsilat.ÜrünTutar}\n", Encoding.UTF8);
                }
                _ = Process.Start(dosyaismi);
            }, parameter => true);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public IEnumerable<Tahsilat> AraçMasraflar { get; set; }

        public ICommand CsvDosyasınaYaz { get; }

        public ObservableCollection<Chart> GrafikVerileri { get; set; }

        public ObservableCollection<İade> Müşteriİadeler { get; set; }

        public IEnumerable<Müşteri> ÖdemeYapmayanMüşteriler { get; set; }

        public double SiparişToplamı { get; set; }

        public IEnumerable<Tahsilat> Tahsilatlar { get; set; }
    }
}