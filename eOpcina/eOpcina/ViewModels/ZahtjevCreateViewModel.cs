using eOpcina.Models;
using System.ComponentModel.DataAnnotations;

namespace eOpcina.ViewModels
{
    public class ZahtjevCreateViewModel
    {
        [Required]
        public Razlog RazlogZahtjeva { get; set; }

        [Required]
        public TipDokumenta TipDokumenta { get; set; }
    }
}