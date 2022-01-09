using Autobus.Model;
using Extensions;
using System;
using System.ComponentModel;
using System.Configuration;
using System.IO;
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