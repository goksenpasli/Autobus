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
                GenerateFileName(out string dosyaismi, out string seperator);
                File.AppendAllText(dosyaismi, $"AY{seperator}TOPLAM TUTAR{seperator}SİPARİŞ TUTAR\n", Encoding.UTF8);
                foreach (Tahsilat tahsilat in Tahsilatlar)
                {
                    File.AppendAllText(dosyaismi, $"{tahsilat.Tarih}{seperator}{tahsilat.ToplamTutar}{seperator}{tahsilat.ÜrünTutar}\n", Encoding.UTF8);
                }
                _ = Process.Start(dosyaismi);
            }, parameter => true);

            İadelerCsvDosyasınaYaz = new RelayCommand<object>(parameter =>
            {
                GenerateFileName(out string dosyaismi, out string seperator);
                File.AppendAllText(dosyaismi, $"AD{seperator}SOYAD{seperator}İADE TUTAR\n", Encoding.UTF8);
                foreach (İade iade in Müşteriİadeler)
                {
                    File.AppendAllText(dosyaismi, $"{iade.Ad}{seperator}{iade.Soyad}{seperator}{iade.Tutar}\n", Encoding.UTF8);
                }
                _ = Process.Start(dosyaismi);
            }, parameter => true);

            ÖdemeYapılmayanCsvDosyasınaYaz = new RelayCommand<object>(parameter =>
            {
                GenerateFileName(out string dosyaismi, out string seperator);
                File.AppendAllText(dosyaismi, $"AD{seperator}SOYAD{seperator}KOLTUK NO{seperator}TELEFON\n", Encoding.UTF8);
                foreach (Müşteri müşteri in ÖdemeYapmayanMüşteriler)
                {
                    File.AppendAllText(dosyaismi, $"{müşteri.Ad}{seperator}{müşteri.Soyad}{seperator}{müşteri.KoltukNo}{seperator}{müşteri.Telefon}\n", Encoding.UTF8);
                }
                _ = Process.Start(dosyaismi);
            }, parameter => true);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public IEnumerable<Tahsilat> AraçMasraflar { get; set; }

        public ICommand CsvDosyasınaYaz { get; }

        public ObservableCollection<Chart> GrafikVerileri { get; set; }

        public ICommand İadelerCsvDosyasınaYaz { get; }

        public ObservableCollection<İade> Müşteriİadeler { get; set; }

        public ICommand ÖdemeYapılmayanCsvDosyasınaYaz { get; }

        public IEnumerable<Müşteri> ÖdemeYapmayanMüşteriler { get; set; }

        public double SiparişToplamı { get; set; }

        public IEnumerable<Tahsilat> Tahsilatlar { get; set; }

        private static void GenerateFileName(out string dosyaismi, out string seperator)
        {
            dosyaismi = Path.GetTempPath() + Guid.NewGuid() + ".csv";
            seperator = new CultureInfo(CultureInfo.CurrentCulture.Name).TextInfo.ListSeparator;
        }
    }
}