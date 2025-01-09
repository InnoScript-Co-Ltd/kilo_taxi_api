using KiloTaxi.Common.Enums;
using KiloTaxi.DataAccess.Interface;
using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.DataAccess.Interface;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Model.DTO;
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

        _hubConnection.On<TripLocation>(
            "ReceiveTripLocation",
            (tripLocation) =>
            {
                Console.WriteLine(
                    $"OrderId: {tripLocation.OrderId}, Latitude: {tripLocation.Lat}, Longitude: {tripLocation.Long}"
                );

                // Create an instance of OrderRouteDTO
                var orderRouteDTO = new OrderRouteDTO
                {
                    OrderId = int.Parse(tripLocation.OrderId),
                    Lat = tripLocation.Lat,
                    Long = tripLocation.Long,
                    CreateDate = DateTime.Now,
                };

                // Use the repository to update the order route
                using var scope = _serviceProvider.CreateScope();
                var tripRepository =
                    scope.ServiceProvider.GetRequiredService<IOrderRouteRepository>();

                var createdOrderRoute= tripRepository.CreateOrderRoute(orderRouteDTO);

                if (createdOrderRoute !=null)
                {
                    Console.WriteLine($"Order route created successfully for OrderId: {tripLocation.OrderId}");
                }
                else
                {
                    Console.WriteLine($"Failed to create order route for OrderId: {tripLocation.OrderId}");
                }
            }
        );
        _hubConnection.On<string>("ReceiveTestMethod", (message) =>
        {
            Console.WriteLine($"Message from server: {message}");
        });

        _hubConnection.On<OrderDTO,int>("AcceptOrderAsync", async(orderDTO, driverID) =>
        {
            Console.WriteLine($"Message from server: accept order");
            using var scope = _serviceProvider.CreateScope();
            var driverRepository = scope.ServiceProvider.GetRequiredService<IDriverRepository>();
            var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
            var driverInfoDTO = driverRepository.GetDriverById(driverID);
            orderDTO.Status = Common.Enums.OrderStatus.InProgress;
            orderRepository.UpdateOrder(orderDTO);
            if (_hubConnection.State == HubConnectionState.Connected)
            {
                await _hubConnection.InvokeAsync("SendDriverInfoToCustomer", orderDTO, driverInfoDTO);
            }
        });
        _serviceProvider = serviceProvider;
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

    public async Task SendOrderAsync(OrderDTO orderDTO, IDriverRepository _driverRepository)
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
