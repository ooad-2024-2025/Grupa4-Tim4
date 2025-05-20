using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eOpcina.Data.Migrations
{
    /// <inheritdoc />
    public partial class eOpcina : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Korisnik",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Ime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prezime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JMBG = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Lozinka = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ElektronskiPotpis = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BrojLicneKarte = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RokTrajanjaLicneKarte = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Spol = table.Column<int>(type: "int", nullable: false),
                    AdresaPrebivalista = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Korisnik", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Korisnik_AspNetUsers_Id",
                        column: x => x.Id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sablon",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TipDokumenta = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sablon", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Dokument",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DatumIzdavanja = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RokTrajanja = table.Column<int>(type: "int", nullable: false),
                    IdSablona = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dokument", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dokument_Sablon_IdSablona",
                        column: x => x.IdSablona,
                        principalTable: "Sablon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Zahtjev",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DatumSlanja = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdKorisnika = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IdDokumenta = table.Column<int>(type: "int", nullable: false),
                    RazlogZahtjeva = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zahtjev", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Zahtjev_Dokument_IdDokumenta",
                        column: x => x.IdDokumenta,
                        principalTable: "Dokument",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Zahtjev_Korisnik_IdKorisnika",
                        column: x => x.IdKorisnika,
                        principalTable: "Korisnik",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Dokument_IdSablona",
                table: "Dokument",
                column: "IdSablona");

            migrationBuilder.CreateIndex(
                name: "IX_Zahtjev_IdDokumenta",
                table: "Zahtjev",
                column: "IdDokumenta");

            migrationBuilder.CreateIndex(
                name: "IX_Zahtjev_IdKorisnika",
                table: "Zahtjev",
                column: "IdKorisnika");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Zahtjev");

            migrationBuilder.DropTable(
                name: "Dokument");

            migrationBuilder.DropTable(
                name: "Korisnik");

            migrationBuilder.DropTable(
                name: "Sablon");
        }
    }
}
