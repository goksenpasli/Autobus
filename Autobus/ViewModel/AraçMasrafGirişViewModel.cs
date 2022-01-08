using Autobus.Model;
using Extensions;
using System.Windows.Input;

namespace Autobus.ViewModel
{
    public class AraçMasrafGirişViewModel
    {
        public AraçMasrafGirişViewModel()
        {
            Masraf = new Masraf();

            MasrafEkle = new RelayCommand<object>(parameter =>
            {
                Masraf masraf = new();
                masraf.Id = ExtensionMethods.RandomNumber();
                masraf.Açıklama = Masraf.Açıklama;
                masraf.SeferId = Masraf.SeferId;
                masraf.Tutar = Masraf.Tutar;

                SeçiliAraç?.Masraf.Add(masraf);
                MainViewModel.DatabaseSave.Execute(null);
            }, parameter => SeçiliSefer is not null && SeçiliAraç is not null && Masraf?.Tutar != 0 && !string.IsNullOrWhiteSpace(Masraf?.Açıklama));
        }

        public Masraf Masraf { get; set; }

        public ICommand MasrafEkle { get; }

        public Araç SeçiliAraç { get; set; }

        public Sefer SeçiliSefer { get; set; }
    }
}