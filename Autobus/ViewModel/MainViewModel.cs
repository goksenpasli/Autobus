using Autobus.Model;
using Extensions;
using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Autobus.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public static readonly string xmldatapath = Path.GetDirectoryName(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath) + @"\Data.xml";

        public MainViewModel()
        {
            Otobüs = new Otobüs
            {
                Araçlar = new Araçlar
                {
                    Araç = ExtensionMethods.AraçlarıYükle(),
                },
                Markalar = new Markalar
                {
                    Marka = ExtensionMethods.MarkalarıYükle(),
                },
                Sefer = ExtensionMethods.SeferleriYükle(),
                Şöförler = new Şöförler
                {
                    Şöför = ExtensionMethods.ŞoförleriYükle()
                },
                Ürünler = new Ürünler
                {
                    Ürün=ExtensionMethods.ÜrünleriYükle()
                }
                
            };

            AraçGirişViewModel = new AraçGirişViewModel();
            SeferGirişViewModel = new SeferGirişViewModel();
            ŞoförGirişViewModel = new ŞoförGirişViewModel();
            YolcuGirişViewModel = new YolcuGirişViewModel();
            YolcuDüzenViewModel = new YolcuDüzenViewModel();
            AraçMasrafGirişViewModel = new AraçMasrafGirişViewModel();
            DatabaseSave = new RelayCommand<object>(parameter => Otobüs.Serialize());
            AraçGirişEkranı = new RelayCommand<object>(parameter => CurrentView = AraçGirişViewModel, parameter => CurrentView != AraçGirişViewModel);
            YolcuGirişEkranı = new RelayCommand<object>(parameter =>
            {
                YolcuGirişViewModel.SeçiliSefer = null;
                CurrentView = YolcuGirişViewModel;
            }, parameter => CurrentView != YolcuGirişViewModel);
            YolcuDüzeniEkranı = new RelayCommand<object>(parameter => CurrentView = YolcuDüzenViewModel, parameter => CurrentView != YolcuDüzenViewModel);
            SeferGirişEkranı = new RelayCommand<object>(parameter => CurrentView = SeferGirişViewModel, parameter => CurrentView != SeferGirişViewModel);
            ŞoförGirişEkranı = new RelayCommand<object>(parameter => CurrentView = ŞoförGirişViewModel, parameter => CurrentView != ŞoförGirişViewModel);
            AraçMasrafEkranı = new RelayCommand<object>(parameter =>
            {
                AraçMasrafGirişViewModel.SeçiliSefer = null;
                CurrentView = AraçMasrafGirişViewModel;
            }, parameter => CurrentView != AraçMasrafGirişViewModel);

            UygulamadanÇık = new RelayCommand<object>(parameter =>
            {
                if (MessageBox.Show("Uygulamadan çıkmak istiyor musun?", App.Current.MainWindow.Title, MessageBoxButton.YesNo, MessageBoxImage.Exclamation, MessageBoxResult.No) == MessageBoxResult.Yes)
                {
                    Application.Current.MainWindow.Close();
                }
            });

            VeritabanınıAç = new RelayCommand<object>(parameter =>
            {
                if (File.Exists(xmldatapath) && MessageBox.Show("Veritabanı dosyasını düzenlemek istiyor musun? Dikkat yanlış düzenleme programın açılmamasına neden olabilir. Devam edilsin mi?", App.Current.MainWindow.Title, MessageBoxButton.YesNo, MessageBoxImage.Exclamation, MessageBoxResult.No) == MessageBoxResult.Yes)
                {
                    _ = Process.Start(xmldatapath);
                }
            });

            PropertyChanged += MainViewModel_PropertyChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public static ICommand DatabaseSave { get; set; }

        public ICommand AraçGirişEkranı { get; }

        public AraçGirişViewModel AraçGirişViewModel { get; set; }

        public ICommand AraçMasrafEkranı { get; }

        public AraçMasrafGirişViewModel AraçMasrafGirişViewModel { get; set; }

        public object CurrentView { get; set; }

        public double Fold { get; set; } = 0.5;

        public Otobüs Otobüs { get; set; }

        public ICommand SeferGirişEkranı { get; }

        public SeferGirişViewModel SeferGirişViewModel { get; set; }

        public ICommand ŞoförGirişEkranı { get; }

        public ŞoförGirişViewModel ŞoförGirişViewModel { get; set; }

        public ICommand UygulamadanÇık { get; }

        public ICommand VeritabanınıAç { get; }

        public ICommand YolcuDüzeniEkranı { get; }

        public YolcuDüzenViewModel YolcuDüzenViewModel { get; set; }

        public ICommand YolcuGirişEkranı { get; }

        public YolcuGirişViewModel YolcuGirişViewModel { get; set; }

        private DispatcherTimer timer;

        private void MainViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is "CurrentView")
            {
                timer = new(DispatcherPriority.Normal) { Interval = TimeSpan.FromMilliseconds(15) };
                Fold = 0.5;
                timer.Tick += OnTick;
                timer.Start();
            }

            void OnTick(object sender, EventArgs e)
            {
                Fold -= 0.01;
                if (Fold <= 0)
                {
                    Fold = 0;
                    timer.Stop();
                    timer.Tick -= OnTick;
                }
            }
        }
    }
}