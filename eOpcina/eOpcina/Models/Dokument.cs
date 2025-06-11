using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eOpcina.Models
{
    public class Dokument
    {
        [Key]
        public int Id { get; set; }
        public DateTime DatumIzdavanja { get; set; }
        public int RokTrajanja { get; set; }
        public byte[] PDFDokumenta { get; set; }
        [ForeignKey("Sablon")]
        public int IdSablona { get; set; }
        public Sablon Sablon { get; set; }

        public Dokument() { }
    }
}
