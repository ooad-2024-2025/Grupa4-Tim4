using System.ComponentModel.DataAnnotations;

namespace eOpcina.Models
{
    public class Sablon
    {
        [Key]
        public int Id { get; set; }
        public TipDokumenta TipDokumenta { get; set; }
        public byte[] PDFSablona { get; set; }

        public Sablon() { }
    }
}
