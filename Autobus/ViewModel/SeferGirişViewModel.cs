using Autobus.Model;
using Extensions;
using System;
using System.Windows.Input;

namespace Autobus.ViewModel
{
    public class SeferGirişViewModel
    {
        public SeferGirişViewModel()
        {
            Sefer = new Sefer();

            SeferEkle = new RelayCommand<object>(parameter =>
            {
                Sefer sefer = new();
                sefer.Id = ExtensionMethods.RandomNumber();
                sefer.BiletTutarı = Math.Round(Sefer.BiletTutarı, 2);
                sefer.KalkışZamanı = Sefer.KalkışZamanı;
                sefer.KalkışŞehirId = Sefer.KalkışŞehirId;
                sefer.AraçId = Sefer.AraçId;
                sefer.TahminiSüre = Sefer.TahminiSüre;
                sefer.VarışZamanı = Sefer.VarışZamanı;
                sefer.VarışŞehirId = Sefer.VarışŞehirId;
                sefer.ŞöförId = Sefer.ŞöförId;

                (parameter as Otobüs)?.Sefer.Add(sefer);
                MainViewModel.DatabaseSave.Execute(null);
            }, parameter => Sefer?.ŞöförId != 0 && Sefer?.AraçId != 0 && Sefer?.TahminiSüre != 0 && Sefer?.BiletTutarı != 0 && Sefer?.KalkışŞehirId != 0 && Sefer?.VarışŞehirId != 0);
        }

        public Sefer Sefer { get; set; }

        public ICommand SeferEkle { get; }
    }
}