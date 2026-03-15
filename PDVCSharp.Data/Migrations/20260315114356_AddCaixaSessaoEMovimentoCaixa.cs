using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PDVCSharp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCaixaSessaoEMovimentoCaixa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CaixaSessoes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DataHoraAbertura = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ValorAbertura = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsOpen = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaixaSessoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CaixaSessoes_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MovimentosCaixa",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DataHora = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CaixaSessaoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Origem = table.Column<int>(type: "int", nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Observacao = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimentosCaixa", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovimentosCaixa_CaixaSessoes_CaixaSessaoId",
                        column: x => x.CaixaSessaoId,
                        principalTable: "CaixaSessoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_CaixaSessoes_UsuarioId",
                table: "CaixaSessoes",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimentosCaixa_CaixaSessaoId",
                table: "MovimentosCaixa",
                column: "CaixaSessaoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MovimentosCaixa");

            migrationBuilder.DropTable(
                name: "CaixaSessoes");
        }
    }
}
