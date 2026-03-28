using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eOpcina.Data.Migrations
{
    /// <inheritdoc />
    public partial class OciscenKorisnikUserModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Zahtjev_Korisnik_IdKorisnika",
                table: "Zahtjev");

            migrationBuilder.DropTable(
                name: "Korisnik");

            migrationBuilder.AddColumn<string>(
                name: "AdresaPrebivalista",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BrojLicneKarte",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ElektronskiPotpis",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Ime",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "JMBG",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Lozinka",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Prezime",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "RokTrajanjaLicneKarte",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Spol",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Zahtjev_AspNetUsers_IdKorisnika",
                table: "Zahtjev",
                column: "IdKorisnika",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Zahtjev_AspNetUsers_IdKorisnika",
                table: "Zahtjev");

            migrationBuilder.DropColumn(
                name: "AdresaPrebivalista",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "BrojLicneKarte",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ElektronskiPotpis",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Ime",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "JMBG",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Lozinka",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Prezime",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RokTrajanjaLicneKarte",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Spol",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "Korisnik",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AdresaPrebivalista = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BrojLicneKarte = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ElektronskiPotpis = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JMBG = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Lozinka = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prezime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RokTrajanjaLicneKarte = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Spol = table.Column<int>(type: "int", nullable: false)
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

            migrationBuilder.AddForeignKey(
                name: "FK_Zahtjev_Korisnik_IdKorisnika",
                table: "Zahtjev",
                column: "IdKorisnika",
                principalTable: "Korisnik",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
