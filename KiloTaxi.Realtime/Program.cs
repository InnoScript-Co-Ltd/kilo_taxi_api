using KiloTaxi.Logging;
using KiloTaxi.Realtime.Helper.ServiceExtensions;
using KiloTaxi.Realtime.Hubs;
using KiloTaxi.Realtime.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin();
        builder.AllowAnyMethod();
        builder.AllowAnyHeader();
    });
});
builder.Services.AddHttpClient();
ConfigHelper.ConfigureService(builder);

builder.Services.AddSingleton<DriverConnectionManager>();
builder.Services.AddSignalR(x =>
{
    x.MaximumReceiveMessageSize = 1000 * 9000;  //9000 KB, 9MB
    x.EnableDetailedErrors = true;
});

LoggerHelper.Instance.LogInfo("Starting the API web host...");

try
{
    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseRouting();
    
    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapHub<DriverHub>("/driver");
        endpoints.MapHub<DashboardHub>("/dashboard");    
        endpoints.MapHub<ApiHub>("/apihub");
    });

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