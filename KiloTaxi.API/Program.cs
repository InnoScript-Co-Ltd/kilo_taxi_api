using System.Security.Claims;
using System.Text;
using KiloTaxi.API;
using KiloTaxi.API.Helper.Authentication.Implementation;
using KiloTaxi.API.Helper.Authentication.Interface;
using KiloTaxi.API.Helper.Filters;
using KiloTaxi.API.Helper.ServiceExtensions;
using KiloTaxi.API.Services;
using KiloTaxi.DataAccess.Implementation;
using KiloTaxi.DataAccess.Interface;
using KiloTaxi.Logging;
using Microsoft.OpenApi.Models;
using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<AdminRepository>(); // If AdminRepository is not registered, add this too
builder.Services.AddScoped<CustomerRepository>();
builder.Services.AddScoped<DriverRepository>();

// Add services to the container.
// Configure JWT Bearer Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        // options.Authority = builder.Configuration["Jwt:Issuer"];
        // options.Audience = builder.Configuration["Jwt:Audience"];
        // options.RequireHttpsMetadata = false;
        // options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // ValidateIssuer = true,
            // ValidateAudience = true,
            // ValidateLifetime = true,
            // ValidateIssuerSigningKey = true,
            // ValidIssuer = builder.Configuration["Jwt:Issuer"],  // Ensure this matches the issuer in BC.OpenIddict
            // ValidAudience = builder.Configuration["Jwt:Audience"],  // Ensure this matches the audience in BC.OpenIddict
            // IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])), // Ensure the secret key matches the one used in BC.OpenIddict
            // SaveSigninToken = true
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"])),
            RoleClaimType = ClaimTypes.Role, // Map Role Claim
            ClockSkew = TimeSpan.Zero

            //ValidIssuer = builder.Configuration["Jwt:Issuer"],  // Ensure this matches the issuer in BC.OpenIddict
            //ValidAudience = builder.Configuration["Jwt:Audience"],  // Ensure this matches the audience in BC.OpenIddict
            //IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])) // Ensure the secret key matches the one used in BC.OpenIddict
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("Token validated successfully.");
                return Task.CompletedTask;
            }
        };
        
    }
    );  
builder.Services.AddDistributedMemoryCache();  // Add in-memory cache
builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(30); // Set the session timeout duration
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });
// Start the SignalR connection on app startup
builder.Services.AddSingleton(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    //var driverRepository = provider.GetRequiredService<IDriverRepository>();
    return new ApiClientHub(configuration, provider);
});

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "KiloTaxi.API", Version = "v1" });
    c.OperationFilter<SwaggerFileUploadOperationFilter>();
});

builder
.Services.AddControllers()
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(
        new System.Text.Json.Serialization.JsonStringEnumConverter()
    );
});


builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin();
        builder.AllowAnyMethod();
        builder.AllowAnyHeader();
    });
});


ConfigHelper.ConfigureService(builder);

LoggerHelper.Instance.LogInfo("Starting the API web host...");

try
{
    var app = builder.Build();

    // Configure the HTTP request pipeline.
    // if (app.Environment.IsDevelopment())
    // {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "KiloTaxi.API V1");
        });
   // }

    app.UseHttpsRedirection();

    app.UseCors();
    
    app.UseSession();
        
    app.UseAuthentication();
    
    app.UseAuthorization();

    app.MapControllers();

    ConfigHelper.MigrateDatabase(app);
    
    var signalRApiClient = app.Services.GetRequiredService<ApiClientHub>();
    await signalRApiClient.StartConnectionAsync();
    
    app.Run();
}
catch (Exception ex)
{
    LoggerHelper.Instance.LogError(ex, "API Host terminated unexpectedly.");
}
finally
{
    LoggerHelper.Instance.LogInfo("Shutting down the API web host...");
    Log.CloseAndFlush();
}
