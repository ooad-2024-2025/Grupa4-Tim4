﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eOpcina.Data.Migrations
{
    /// <inheritdoc />
    public partial class IzbrisanLozinkaPropertyKorisnik : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Lozinka",
                table: "AspNetUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Lozinka",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
