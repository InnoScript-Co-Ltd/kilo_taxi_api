using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KiloTaxi.DataAccess.Implementation;
using KiloTaxi.DataAccess.Interface;
using KiloTaxi.EntityFramework.EntityModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KiloTaxi.DataAccess.Helper
{
    public class DataAccessService
    {
        public static void ConfigureServices(
            IServiceCollection services,
            IConfiguration Configuration
        )
        {
            services.AddScoped<IDriverRepository,DriverRepository>();
            services.AddScoped<IVehicleRepository,VehicleRepository>();
            services.AddScoped<IAdminRepository, AdminRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IPromotionRepository, PromotionRepository>();
            services.AddScoped<IReviewRepository, ReviewRepository>();
        }
    }
}
