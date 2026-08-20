using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PDVCSharp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSkuCategoriaAndVendaCaixa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Sku",
                table: "Produtos",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Categoria",
                table: "Produtos",
                type: "varchar(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "Geral")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_Sku",
                table: "Produtos",
                column: "Sku");

            migrationBuilder.AddColumn<Guid>(
                name: "CaixaSessaoId",
                table: "Vendas",
                type: "char(36)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vendas_CaixaSessaoId",
                table: "Vendas",
                column: "CaixaSessaoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vendas_CaixaSessoes_CaixaSessaoId",
                table: "Vendas",
                column: "CaixaSessaoId",
                principalTable: "CaixaSessoes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vendas_CaixaSessoes_CaixaSessaoId",
                table: "Vendas");

            migrationBuilder.DropIndex(
                name: "IX_Vendas_CaixaSessaoId",
                table: "Vendas");

            migrationBuilder.DropColumn(
                name: "CaixaSessaoId",
                table: "Vendas");

            migrationBuilder.DropIndex(
                name: "IX_Produtos_Sku",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "Sku",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "Categoria",
                table: "Produtos");
        }
    }
}
