using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eOpcina.Data.Migrations
{
    /// <inheritdoc />
    public partial class DodanoZakljucanPolje : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Zakljucan",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Zakljucan",
                table: "AspNetUsers");
        }
    }
}
