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
        }
    }
}
