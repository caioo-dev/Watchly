using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Watchly.Migrations
{
    /// <inheritdoc />
    public partial class AlteracaoTabela : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Rating",
                table: "UsuarioTitulos",
                newName: "Nota");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Nota",
                table: "UsuarioTitulos",
                newName: "Rating");
        }
    }
}
