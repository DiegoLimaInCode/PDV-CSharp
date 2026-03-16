using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PDVCSharp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLoginOperadorToMovimentoCaixa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LoginOperador",
                table: "MovimentosCaixa",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoginOperador",
                table: "MovimentosCaixa");
        }
    }
}
