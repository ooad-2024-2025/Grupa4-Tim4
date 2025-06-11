using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eOpcina.Models
{
    public class Zahtjev
    {
        [Key]
        public int Id { get; set; }
        public DateTime DatumSlanja { get; set; }
        [ForeignKey("Korisnik")]
        public string IdKorisnika { get; set; }
        [ForeignKey("Dokument")]
        public int IdDokumenta { get; set; }
        public Razlog RazlogZahtjeva { get; set; }
        public Korisnik Korisnik { get; set; }
        public Dokument Dokument { get; set; }

        public Zahtjev() { }
    }
}