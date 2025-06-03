using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace eOpcina.Models
{
    public class Korisnik : IdentityUser
    {
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string JMBG { get; set; }
        public string ElektronskiPotpis { get; set; }
        public string BrojLicneKarte { get; set; }
        public DateTime RokTrajanjaLicneKarte { get; set; }
        public Spol Spol { get; set; }
        public string AdresaPrebivalista { get; set; }
        public Korisnik() { }
    }
}
