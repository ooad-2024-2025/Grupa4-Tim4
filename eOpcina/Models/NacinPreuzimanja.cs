using System.ComponentModel.DataAnnotations;

namespace eOpcina.Models
{
    public enum NacinPreuzimanja
    {
        [Display(Name = "Preko maila")]
        PrekoMaila,
        [Display(Name = "Lično")]
        Licno
    }
}
