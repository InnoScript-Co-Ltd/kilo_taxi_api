using KiloTaxi.Common.Enums;
using KiloTaxi.DataAccess.Interface;
using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace KiloTaxi.API.Services;

public class ApiClientHub : IDisposable
{
    private readonly HubConnection _hubConnection;
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;

    public ApiClientHub(IConfiguration configuration, IServiceProvider serviceProvider)
    {
        _configuration = configuration;
        _serviceProvider = serviceProvider;

        string signalrApiHubUrl = _configuration["SignalrApiHubUrl"];

        _hubConnection = new HubConnectionBuilder()
            .WithUrl(signalrApiHubUrl)
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.Reconnecting += async (error) =>
        {
            Console.WriteLine("Reconnecting to SignalR server...");
            await Task.CompletedTask;
        };

        _hubConnection.Closed += async (error) =>
        {
            Console.WriteLine("Connection closed. Attempting to restart...");
            await StartConnectionAsync();
        };

        // Add handlers for incoming messages
        _hubConnection.On<string>(
            "ReceiveTestMethod",
            (message) =>
            {
                Console.WriteLine($"Message from server: {message}");
            }
        );
        _hubConnection.On<string, int>(
            "ReceiveAvailityStatus",
            (availityStatus, driverId) =>
            {
                Console.WriteLine("status");
                using var scope = _serviceProvider.CreateScope();
                var driverRepository =
                    scope.ServiceProvider.GetRequiredService<IDriverRepository>();

                DriverCreateFormDTO driverCreateFormDto = new DriverCreateFormDTO();
                driverCreateFormDto.Id = driverId;
                driverCreateFormDto.AvailableStatus = Enum.Parse<DriverStatus>(availityStatus);
                driverRepository.UpdateDriverStatus(driverCreateFormDto);
            }
        );

        _hubConnection.On<int, double, double>(
            "ReceiveTripLocation",
            (orderId, latitude, longitude) =>
            {
                Console.WriteLine(
                    $"OrderId: {orderId}, Latitude: {latitude}, Longitude: {longitude}"
                );

                // Create an instance of OrderRouteDTO
                var orderRouteDTO = new OrderRouteDTO
                {
                    OrderId = orderId,
                    Lat = latitude.ToString(),
                    Long = longitude.ToString(),
                    CreateDate = DateTime.UtcNow,
                };

                // Use the repository to update the order route
                using var scope = _serviceProvider.CreateScope();
                var tripRepository =
                    scope.ServiceProvider.GetRequiredService<IOrderRouteRepository>();

                bool isUpdated = tripRepository.UpdateOrderRoute(orderRouteDTO);

                if (isUpdated)
                {
                    Console.WriteLine($"Order route updated successfully for OrderId: {orderId}");
                }
                else
                {
                    Console.WriteLine($"Failed to update order route for OrderId: {orderId}");
                }
            }
        );
    }

    public async Task StartConnectionAsync()
    {
        try
        {
            await _hubConnection.StartAsync();
            Console.WriteLine("Connected to SignalR server.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to connect: {ex.Message}");
        }
    }

    public async Task SendMessageAsync(string message)
    {
        if (_hubConnection.State == HubConnectionState.Connected)
        {
            await _hubConnection.InvokeAsync("SendMessage", message);
        }
    }

    public async Task SendOrderAsync(OrderDTO orderDTO)
    {
        using var scope = _serviceProvider.CreateScope();
        var driverRepository = scope.ServiceProvider.GetRequiredService<IDriverRepository>();
        var onlineDriverDTOList = driverRepository.SearchNearbyOnlineDriver();
        if (_hubConnection.State == HubConnectionState.Connected)
        {
            await _hubConnection.InvokeAsync("SendOrder", orderDTO, onlineDriverDTOList);
        }
    }

    public void Dispose()
    {
        _hubConnection?.DisposeAsync();
    }
}
