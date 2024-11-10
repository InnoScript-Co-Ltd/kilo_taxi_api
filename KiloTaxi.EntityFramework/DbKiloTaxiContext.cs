using KiloTaxi.EntityFramework.EntityModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiloTaxi.EntityFramework
{
    public class DbKiloTaxiContext : DbContext
    {
        public DbKiloTaxiContext(DbContextOptions<DbKiloTaxiContext> options)
            :base(options){ }

        public DbSet<Admin> Admins { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admin>().ToTable("Admin");
        }
    }
}
