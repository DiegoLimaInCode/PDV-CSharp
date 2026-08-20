using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PDVCSharp.Data.Context
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            var connectionString = Environment.GetEnvironmentVariable("PDV_CONNECTION")
                ?? "Data Source=pdv.db";
            optionsBuilder.UseSqlite(connectionString);
            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
