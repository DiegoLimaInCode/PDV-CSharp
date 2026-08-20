using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using PDVCSharp.Data.Context;

#nullable disable

namespace PDVCSharp.Data.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20260815120000_AddSkuCategoriaAndVendaCaixa")]
    partial class AddSkuCategoriaAndVendaCaixa
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
        }
    }
}
