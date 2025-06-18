using eOpcina.Models;
using System.ComponentModel.DataAnnotations;

namespace eOpcina.ViewModels
{


    public class ZahtjevCreateViewModel
    {
        [Required]
        [Display(Name = "Tip dokumenta")]
        public TipDokumenta TipDokumenta { get; set; }

        [Required]
        [Display(Name = "Razlog zahtjeva")]
        public Razlog RazlogZahtjeva { get; set; }

        [Required]
        public NacinPreuzimanja? NacinPreuzimanja { get; set; }

        [Required(ErrorMessage = "Elektronski potpis je obavezan.")]
        [Display(Name = "Elektronski potpis")]
        public string ElektronskiPotpis { get; set; }
    }
}