using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Watchly.Migrations
{
    /// <inheritdoc />
    public partial class ExternalTitulos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalId",
                table: "Titulos",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Fonte",
                table: "Titulos",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImagemUrl",
                table: "Titulos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Titulos_ExternalId_Fonte",
                table: "Titulos",
                columns: new[] { "ExternalId", "Fonte" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Titulos_ExternalId_Fonte",
                table: "Titulos");

            migrationBuilder.DropColumn(
                name: "ExternalId",
                table: "Titulos");

            migrationBuilder.DropColumn(
                name: "Fonte",
                table: "Titulos");

            migrationBuilder.DropColumn(
                name: "ImagemUrl",
                table: "Titulos");
        }
    }
}
