using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KiloTaxi.DataAccess.Implementation;
using KiloTaxi.DataAccess.Interface;

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

        }
    }
}
