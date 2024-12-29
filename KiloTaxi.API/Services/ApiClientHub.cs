using Microsoft.AspNetCore.SignalR.Client;

namespace KiloTaxi.API.Services;

public class ApiClientHub : IDisposable
{
    private readonly HubConnection _hubConnection;
    private readonly IConfiguration _configuration;

    public ApiClientHub(IConfiguration configuration)
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

    public void Dispose()
    {
        _hubConnection?.DisposeAsync();
    }
}