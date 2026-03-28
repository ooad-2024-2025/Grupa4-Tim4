using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace eOpcina.Models
{
    public enum Razlog
    {
        [Display(Name = "Apliciranje na konkurs")]
        ApliciranjeNaKonkurs,
        [Display(Name = "Apliciranje na stipendiju")]
        ApliciranjeNaStipendiju,
        [Display(Name = "Izdavanje lične karte")]
        IzdavanjeLicneKarte,
        [Display(Name = "Izdavanje vozačke dozvole")]
        IzdavanjeVozackeDozvole,
        [Display(Name = "Upis u obrazovnu ustanovu")]
        UpisUObrazovnuUstanovu,
        [Display(Name = "Ostvarivanje dječijeg doplatka")]
        OstvarivanjeDjecijegDoplatka,
        [Display(Name = "Izgubljen dokument")]
        IzgubljenDokument,
        [Display(Name = "Prepis imovine")]
        PrepisImovine
    }
}
