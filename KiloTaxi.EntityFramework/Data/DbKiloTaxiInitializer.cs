using KiloTaxi.Common.Enums;
using KiloTaxi.EntityFramework.EntityModel;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiloTaxi.EntityFramework.Data
{
    public static class DbKiloTaxiInitializer
    {
        public static void Initialize(DbKiloTaxiContext context)
        {
            if (!context.Admins.Any())
            {
                Admin[] admins = new Admin[] {
                    new Admin{Name="Admin1",Phone="095012345",Email="test@email",Password=BCrypt.Net.BCrypt.HashPassword("123456"),Gender=GenderType.Male.ToString(),Address="TestAddress",Status=OtpStatus.Pending.ToString(),Role = "Admin",RefreshToken = "qweqwe",RefreshTokenExpiryTime = DateTime.Now.AddHours(1),Otp = "123456"},
                    new Admin{Name="Admin2",Phone="095012346",Email="test2@email",Password=BCrypt.Net.BCrypt.HashPassword("123456"),Gender=GenderType.Female.ToString(),Address="TestAddress",Status=OtpStatus.Pending.ToString(),Role = "Admin",RefreshToken = "qweqwe",RefreshTokenExpiryTime = DateTime.Now.AddHours(1),Otp = "123456"},
                };
                context.Admins.AddRange(admins);
                context.SaveChanges();
            }
            if (!context.Wallets.Any())
            {
                Wallet[] wallets = new Wallet[]
                {
                    new Wallet {WalletName = "Kilo Normal",CreatedDate = DateTime.Now},
                    new Wallet {WalletName = "Kilo Plus",CreatedDate = DateTime.Now}
                };
                context.Wallets.AddRange(wallets);
                context.SaveChanges();            
            }
            if (!context.Orders.Any())
            {
                Order[] orders = new Order[]
                {
                    new Order {EstimatedAmount= 8500,Status=OrderStatus.Pending.ToString(),PickUpLocation="Lanmadaw",
                        PickUpLat="16.7722",PickUpLong = "96.1489",DestinationLocation = "Hledan",DestinationLat = "16.8245",DestinationLong = "96.1385",CustomerId = 1,CreatedDate = DateTime.Now},
                };
                context.Orders.AddRange(orders);
                context.SaveChanges();            
            }
            if (!context.OrderRoutes.Any())
            {
                OrderRoute[] orderRoutes = new OrderRoute[]
                {
                    new OrderRoute {Lat = "16.7722",Long = "96.1489",OrderId = 1,CreateDate = DateTime.Now},
                    new OrderRoute { Lat= "16.7765", Long= "96.1472",OrderId = 1,CreateDate = DateTime.Now}, 
                    new OrderRoute { Lat= "16.7840", Long = "96.1435",OrderId = 1,CreateDate = DateTime.Now }, 
                    new OrderRoute  { Lat="16.7956", Long ="96.1413",OrderId = 1,CreateDate = DateTime.Now },
                    new OrderRoute  { Lat= "16.8120", Long = "96.1400" ,OrderId = 1,CreateDate = DateTime.Now}, 
                    new OrderRoute { Lat= "16.8245", Long = "96.1385" ,OrderId = 1,CreateDate = DateTime.Now} 
                };
                context.OrderRoutes.AddRange(orderRoutes);
                context.SaveChanges();            
            }
            if (!context.KiloAmounts.Any())
            {
                KiloAmount[] kiloAmounts = new KiloAmount[]
                {
                    new KiloAmount {Kilo = 1,Amount = 1000},
                };
                context.KiloAmounts.AddRange(kiloAmounts);
                context.SaveChanges();            
            }
            if (!context.WaitingDefaults.Any())
            {
                WaitingDefault[] waitingDefaults = new WaitingDefault[]
                {
                    new WaitingDefault {DefaultTime = 10},
                };
                context.WaitingDefaults.AddRange(waitingDefaults);
                context.SaveChanges();            
            }
        }
    }
}
