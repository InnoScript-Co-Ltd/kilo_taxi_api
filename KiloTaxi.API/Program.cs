using KiloTaxi.API.Helper.Filters;
using KiloTaxi.API.Helper.ServiceExtensions;
using KiloTaxi.Logging;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

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
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "KiloTaxi.API V1");
        });
    }

    app.UseHttpsRedirection();

    app.UseCors();

    app.UseAuthorization();

    app.MapControllers();

    ConfigHelper.MigrateDatabase(app);

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
