using LjussAPI.Persistence;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJussAPI.Persistance
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseNpgsql(
                "Host=ecommerce-postgres.postgres.database.azure.com;Database=ECommerceDb;Username=postgres;Password=Tugba.1993;SSL Mode=Require;Trust Server Certificate=true;"
            );

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
