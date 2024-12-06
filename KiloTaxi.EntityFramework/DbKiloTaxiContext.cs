using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KiloTaxi.Common.Enums;
using KiloTaxi.EntityFramework.EntityModel;
using Microsoft.EntityFrameworkCore;

namespace KiloTaxi.EntityFramework
{
    public class DbKiloTaxiContext : DbContext
    {
        public DbKiloTaxiContext(DbContextOptions<DbKiloTaxiContext> options)
            : base(options) { }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Wallet> Wallets { get; set; }

        // public DbSet<PaymentChannel> PaymentChannels { get; set; }
        public DbSet<TopUpTransaction> TopUpTransactions { get; set; }
        public DbSet<WalletUserMapping> WalletUserMappings { get; set; }

        public DbSet<WalletTransaction> WalletTransactions { get; set; }

        public DbSet<PromotionUsage> PromotionUsages { get; set; }

        public DbSet<ScheduleBooking> ScheduleBookings { get; set; }
        
        public DbSet<Sos> Sos { get; set; }
        
        public DbSet<Sms> Sms { get; set; }
        
        public DbSet<Reason> Reasons { get; set; }
        
        public DbSet<Notification> Notifications { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admin>().ToTable("Admin");
            modelBuilder.Entity<Driver>().ToTable("Driver");
            modelBuilder.Entity<Vehicle>().ToTable("Vehicle");
            modelBuilder.Entity<Customer>().ToTable("Customer");
            ;
            modelBuilder.Entity<Promotion>().ToTable("Promotion");
            modelBuilder.Entity<Review>().ToTable("Review");
            modelBuilder.Entity<Order>().ToTable("Order");
            modelBuilder.Entity<Wallet>().ToTable("Wallet");
            modelBuilder.Entity<PaymentChannel>().ToTable("PaymentChannel");
            modelBuilder.Entity<TopUpTransaction>().ToTable("TopUpTransaction");
            modelBuilder.Entity<WalletUserMapping>().ToTable("WalletUserMapping");
            modelBuilder.Entity<WalletTransaction>().ToTable("WalletTransaction");
            modelBuilder.Entity<PromotionUsage>().ToTable("PromotionUsage");
            modelBuilder.Entity<ScheduleBooking>().ToTable("ScheduleBooking");
            modelBuilder.Entity<Reason>().ToTable("Reason");
            modelBuilder.Entity<Sms>().ToTable("Sms");
            modelBuilder.Entity<Sos>().ToTable("Sos");
            modelBuilder.Entity<Notification>().ToTable("Notification");
        }
    }
}
