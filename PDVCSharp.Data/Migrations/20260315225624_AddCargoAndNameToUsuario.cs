using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PDVCSharp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCargoAndNameToUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Cargo",
                table: "Usuarios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Usuarios",
                type: "varchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cargo",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Usuarios");
        }
    }
}
