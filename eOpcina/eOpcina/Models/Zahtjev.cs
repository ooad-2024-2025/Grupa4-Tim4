using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eOpcina.Models
{
    public class Zahtjev
    {
        [Key]
        public int Id { get; set; }
        [DisplayName("Datum slanja")]
        public DateTime DatumSlanja { get; set; }
        [ForeignKey("Korisnik")]
        [DisplayName("ID korisnika")]
        public string IdKorisnika { get; set; }
        [ForeignKey("Dokument")]
        [DisplayName("ID dokumenta")]
        public int IdDokumenta { get; set; }
        [DisplayName("Razlog zahtjeva")]
        public Razlog RazlogZahtjeva { get; set; }
        public StanjeZahtjeva StanjeZahtjeva { get; set; }
        public Korisnik Korisnik { get; set; }
        public Dokument Dokument { get; set; }

        public Zahtjev() { }
    }
}