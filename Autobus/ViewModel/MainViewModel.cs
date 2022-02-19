using Autobus.Model;
using Autobus.Properties;
using Extensions;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
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
                Aylar = ExtensionMethods.AylarıYükle(),
                Ürünler = new Ürünler
                {
                    Ürün = ExtensionMethods.ÜrünleriYükle()
                },
                İadeler = new İadeler
                {
                    İade = ExtensionMethods.İadeleriYükle(),
                }
            };

            AraçGirişViewModel = new AraçGirişViewModel();
            SeferGirişViewModel = new SeferGirişViewModel();
            ŞoförGirişViewModel = new ŞoförGirişViewModel();
            YolcuGirişViewModel = new YolcuGirişViewModel();
            YolcuDüzenViewModel = new YolcuDüzenViewModel();
            TümSeferlerViewModel = new TümSeferlerViewModel();
            ÖdemeYapılmayanKoltuklarViewModel = new ÖdemeYapılmayanKoltuklarViewModel();
            AraçMasrafGirişViewModel = new AraçMasrafGirişViewModel();

            AraçGirişEkranı = new RelayCommand<object>(parameter => CurrentView = AraçGirişViewModel, parameter => CurrentView != AraçGirişViewModel);
            YolcuGirişEkranı = new RelayCommand<object>(parameter =>
            {
                YolcuGirişViewModel.SeçiliSefer = null;
                CurrentView = YolcuGirişViewModel;
            }, parameter => CurrentView != YolcuGirişViewModel);
            YolcuDüzeniEkranı = new RelayCommand<object>(parameter =>
            {
                YolcuDüzenViewModel.SeçiliSefer = null;
                CurrentView = YolcuDüzenViewModel;
            }, parameter => CurrentView != YolcuDüzenViewModel);
            SeferGirişEkranı = new RelayCommand<object>(parameter => CurrentView = SeferGirişViewModel, parameter => CurrentView != SeferGirişViewModel);
            ŞoförGirişEkranı = new RelayCommand<object>(parameter => CurrentView = ŞoförGirişViewModel, parameter => CurrentView != ŞoförGirişViewModel);
            TümSeferlerEkranı = new RelayCommand<object>(parameter => CurrentView = TümSeferlerViewModel, parameter => CurrentView != TümSeferlerViewModel);
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

            DatabaseSave = new RelayCommand<object>(parameter => Otobüs.Serialize());

            VeritabanınıAç = new RelayCommand<object>(parameter =>
            {
                if (File.Exists(xmldatapath) && MessageBox.Show("Veritabanı dosyasını düzenlemek istiyor musun? Dikkat yanlış düzenleme programın açılmamasına neden olabilir. Devam edilsin mi?", App.Current.MainWindow.Title, MessageBoxButton.YesNo, MessageBoxImage.Exclamation, MessageBoxResult.No) == MessageBoxResult.Yes)
                {
                    _ = Process.Start(xmldatapath);
                }
            });

            ÖdemeYapılmayanKoltuklarEkranıAç = new RelayCommand<object>(parameter =>
            {
                ÖdemeYapılmayanKoltuklarViewModel.Tahsilatlar = Otobüs.BiletTahsilat();
                ÖdemeYapılmayanKoltuklarViewModel.AraçMasraflar = Otobüs.AraçMasraflar();
                ÖdemeYapılmayanKoltuklarViewModel.Müşteriİadeler = Otobüs.İadeler.İade;
                ÖdemeYapılmayanKoltuklarViewModel.GrafikVerileri = Otobüs.GrafikVerileri();
                ÖdemeYapılmayanKoltuklarViewModel.ÖdemeYapmayanMüşteriler = Otobüs?.Sefer?.SelectMany(z => z.Müşteri)?.Where(z => !z.BiletÖdendi);
            });

            WebAdreseGit = new RelayCommand<object>(parameter => Process.Start(parameter as string), parameter => true);

            ArşivAç = new RelayCommand<object>(parameter =>
            {
                OpenFileDialog openFileDialog = new() { Multiselect = false, Filter = "Zip Dosyaları (*.zip)|*.zip" };
                if (openFileDialog.ShowDialog() == true)
                {
                    ArşivYolu = openFileDialog.FileName;
                }
            });

            Yedekle = new RelayCommand<object>(parameter =>
            {
                SaveFileDialog saveFileDialog = new()
                {
                    Title = "SAKLA",
                    Filter = "Zip Dosyası (*.zip)|*.zip",
                };
                if (saveFileDialog.ShowDialog() == true)
                {
                    if (File.Exists(saveFileDialog.FileName))
                    {
                        File.Delete(saveFileDialog.FileName);
                    }
                    if (Compress)
                    {
                        ZipFile.CreateFromDirectory(Path.GetDirectoryName(xmldatapath), saveFileDialog.FileName, CompressionLevel.Fastest, false);
                    }
                    else
                    {
                        ZipFile.CreateFromDirectory(Path.GetDirectoryName(xmldatapath), saveFileDialog.FileName, CompressionLevel.NoCompression, false);
                    }
                }
            }, parameter => File.Exists(xmldatapath));

            DefaultScreen = new Dictionary<int, object>
            {
                [0] = AraçGirişViewModel,
                [1] = YolcuGirişViewModel,
                [2] = TümSeferlerViewModel
            };

            if (Settings.Default.EkranSeç)
            {
                CurrentView = DefaultScreen[Settings.Default.VarsayılanEkran];
            }

            if (!File.Exists(xmldatapath))
            {
                _ = Directory.CreateDirectory(Path.GetDirectoryName(xmldatapath));
            }

            TümSeferlerViewModel.Otobüs = Otobüs;
            TümSeferlerViewModel.Seferler = Otobüs?.Sefer;

            Settings.Default.PropertyChanged += Default_PropertyChanged;
            PropertyChanged += MainViewModel_PropertyChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public static ICommand DatabaseSave { get; set; }

        public static Dictionary<int, object> DefaultScreen { get; set; }

        public ICommand AraçGirişEkranı { get; }

        public AraçGirişViewModel AraçGirişViewModel { get; set; }

        public ICommand AraçMasrafEkranı { get; }

        public AraçMasrafGirişViewModel AraçMasrafGirişViewModel { get; set; }

        public RelayCommand<object> ArşivAç { get; }

        public string ArşivYolu { get; set; }

        public bool Compress { get; set; }

        public object CurrentView { get; set; }

        public double Fold { get; set; } = 0.5;

        public Otobüs Otobüs { get; set; }

        public ICommand ÖdemeYapılmayanKoltuklarEkranıAç { get; }

        public ÖdemeYapılmayanKoltuklarViewModel ÖdemeYapılmayanKoltuklarViewModel { get; set; }

        public double Ripple { get; set; }

        public ICommand SeferGirişEkranı { get; }

        public SeferGirişViewModel SeferGirişViewModel { get; set; }

        public ICommand ŞoförGirişEkranı { get; }

        public ŞoförGirişViewModel ŞoförGirişViewModel { get; set; }

        public ICommand TümSeferlerEkranı { get; }

        public TümSeferlerViewModel TümSeferlerViewModel { get; set; }

        public ICommand UygulamadanÇık { get; }

        public ICommand VeritabanınıAç { get; }

        public RelayCommand<object> WebAdreseGit { get; }

        public bool WebPFilesExists { get; set; } = File.Exists(ExeFolder + """\libwebp_x64.dll""") && File.Exists(ExeFolder + """\libwebp_x86.dll""");

        public RelayCommand<object> Yedekle { get; }

        public ICommand YolcuDüzeniEkranı { get; }

        public YolcuDüzenViewModel YolcuDüzenViewModel { get; set; }

        public ICommand YolcuGirişEkranı { get; }

        public YolcuGirişViewModel YolcuGirişViewModel { get; set; }

        private DispatcherTimer timer;

        private static string ExeFolder { get; } = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

        private void Default_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Fold = 0;
            Ripple = 0;
            Settings.Default.Save();
        }

        private void MainViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is "CurrentView")
            {
                timer = new(DispatcherPriority.Normal) { Interval = TimeSpan.FromMilliseconds(15) };
                Fold = 0.5;
                Ripple = 0;
                timer.Tick += OnTick;
                timer.Start();
            }

            void OnTick(object sender, EventArgs e)
            {
                switch (Settings.Default.AnimasyonTipi)
                {
                    case (int)AnimationType.Fold:
                        {
                            Fold -= 0.01;
                            if (Fold <= 0)
                            {
                                Fold = 0;
                                timer.Stop();
                                timer.Tick -= OnTick;
                            }

                            break;
                        }
                    case (int)AnimationType.Ripple:
                        {
                            Ripple++;
                            if (Ripple > 100)
                            {
                                Ripple = 0;
                                timer.Stop();
                                timer.Tick -= OnTick;
                            }

                            break;
                        }
                }
            }
        }
    }
}