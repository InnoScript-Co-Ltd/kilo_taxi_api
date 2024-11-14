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
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admin>().ToTable("Admin");
            modelBuilder.Entity<Driver>().ToTable("Driver");
            modelBuilder.Entity<Vehicle>().ToTable("Vehicle");
            modelBuilder.Entity<Order>().ToTable("Order");

        }
    }
}
