using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace eOpcina.Models
{
    public class Korisnik : IdentityUser
    {
        [Required(ErrorMessage = "Polje Ime je obavezno!")]
        public string Ime { get; set; }
        [Required(ErrorMessage = "Polje Prezime je obavezno!")]
        public string Prezime { get; set; }
        [Required(ErrorMessage = "Polje JMBG je obavezno!")]
        public string JMBG { get; set; }
        [Required(ErrorMessage = "Polje Elektronski potpis je obavezno!")]
        public string ElektronskiPotpis { get; set; }
        [Required(ErrorMessage = "Polje Broj lične karte je obavezno!")]
        public string BrojLicneKarte { get; set; }
        [Required(ErrorMessage = "Polje Rok trajanja lične karte je obavezno!")]
        public DateTime RokTrajanjaLicneKarte { get; set; }
        [Required(ErrorMessage = "Polje Spol je obavezno!")]
        public Spol Spol { get; set; }
        [Required(ErrorMessage = "Polje Adresa prebivališta je obavezno!")]
        public string AdresaPrebivalista { get; set; }
        public Korisnik() { }
    }
}
