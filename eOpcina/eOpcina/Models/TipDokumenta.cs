using System.ComponentModel.DataAnnotations;

namespace eOpcina.Models
{
    public enum TipDokumenta
    {
        [Display(Name = "Cips")]
        Cips,
        [Display(Name = "Državljanstvo")]
        Drzavljanstvo,
        [Display(Name = "Rodni list")]
        RodniList,
        [Display(Name = "Smrtni list")]
        SmrtniList,
        [Display(Name = "Upis novorođenčeta")]
        UpisNovorodjenceta
    }
}