﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KiloTaxi.DataAccess.Implementation;
using KiloTaxi.DataAccess.Interface;
using KiloTaxi.EntityFramework.EntityModel;
using Microsoft.AspNetCore.Authentication;
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
            services.AddScoped<IDriverRepository, DriverRepository>();
            services.AddScoped<IVehicleRepository, VehicleRepository>();
            services.AddScoped<IVehicleTypeRepository, VehicleTypeRepository>();
            services.AddScoped<IAdminRepository, AdminRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IPromotionRepository, PromotionRepository>();
            services.AddScoped<IReviewRepository, ReviewRepository>();
            services.AddScoped<IWalletRepository, WalletRepository>();
            services.AddScoped<IWalletUserMappingRepository, WalletUserMappingRepository>();
            services.AddScoped<ITopUpTransactionRepository, TopUpTransactionRepository>();
            services.AddScoped<IWalletTransactionRepository, WalletTransactionRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IScheduleBookingRepository, ScheduleBookingRepository>();
            services.AddScoped<ISmsRepository, SmsRepository>();
            services.AddScoped<ISosRepository, SosRepository>();
            services.AddScoped<IReasonRepository, ReasonRepository>();
            services.AddScoped<IPaymentChannelRepository, PaymentChannelRepository>();
            services.AddScoped<ITransactionLogRepository, TransactionLogRepository>();
            services.AddScoped<ICityRepository, CityRepository>();
            services.AddScoped<ITravelRateRepository, TravelRateRepository>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IOrderRouteRepository, OrderRouteRepository>();
            services.AddScoped<IOrderExtendRepository, OrderExtendRepository>();
            services.AddScoped<IExtraDemandRepository, ExtraDemandRepository>();
            services.AddScoped<IOrderExtraDemandRepository, OrderExtraDemandRepository>();
        }
    }
}
