using KiloTaxi.EntityFramework.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiloTaxi.EntityFramework.Helper
{
    public static class DbKiloTaxiService
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration Configuration, string connectionString)
        {
            services.AddDbContext<DbKiloTaxiContext>(options =>
                options.UseSqlServer($"name=ConnectionStrings:{connectionString}"));

            services.AddDatabaseDeveloperPageExceptionFilter();

        }

        public static void MigrateDatabase(IServiceScope scope)
        {
            var dbContextOptions = scope.ServiceProvider.GetRequiredService<DbKiloTaxiContext>();
            dbContextOptions.Database.Migrate();
            DbKiloTaxiInitializer.Initialize(dbContextOptions);
        }

        public static void DbEnsureCreate(IServiceScope scope)
        {
            var context = scope.ServiceProvider.GetRequiredService<DbKiloTaxiContext>();
            context.Database.EnsureCreated();
        }
    }
}
