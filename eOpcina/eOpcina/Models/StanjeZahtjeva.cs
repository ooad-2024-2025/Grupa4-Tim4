using System.ComponentModel.DataAnnotations;

namespace eOpcina.Models
{
    public enum StanjeZahtjeva
    {
        [Display(Name = "Poslan")]
        Poslan,
        [Display(Name = "Obrađen")]
        Obradjen,
        [Display(Name = "Može se preuzeti")]
        MozeSePreuzeti,
        [Display(Name = "Preuzet")]
        Preuzet
    }
}
