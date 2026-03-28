using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eOpcina.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedPDFToModels_TarikB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "PDFSablona",
                table: "Sablon",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "PDFDokumenta",
                table: "Dokument",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PDFSablona",
                table: "Sablon");

            migrationBuilder.DropColumn(
                name: "PDFDokumenta",
                table: "Dokument");
        }
    }
}
