using KiloTaxi.DataAccess.Interface;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Response;
using Microsoft.AspNetCore.SignalR.Client;

namespace KiloTaxi.API.Services;

public class ApiClientHub : IDisposable
{
    private readonly HubConnection _hubConnection;
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;

    public ApiClientHub(IConfiguration configuration, IServiceProvider serviceProvider)
    {
        _configuration = configuration;

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
        var onlineDriverDTOList = _driverRepository.SearchNearbyOnlineDriver();
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