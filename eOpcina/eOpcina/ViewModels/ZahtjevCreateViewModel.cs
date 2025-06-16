using eOpcina.Models;
using System.ComponentModel.DataAnnotations;

namespace eOpcina.ViewModels
{
    public enum NacinPreuzimanja
    {
        PrekoMaila,
        UOpcini
    }

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
    }
}