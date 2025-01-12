using KiloTaxi.Logging;
using KiloTaxi.Realtime.HubInterfaces;
using KiloTaxi.Realtime.Services;
using Microsoft.AspNetCore.SignalR;

namespace KiloTaxi.Realtime.Hubs;

public class CustomerHub : Hub<ICustomerClient>, ICustomerHub
{
    LoggerHelper _logHelper;
    private CustomerConnectionManager _customerConnectionManager;

    public CustomerHub(CustomerConnectionManager customerConnectionManager)
    {
        _logHelper = LoggerHelper.Instance;
        _customerConnectionManager = customerConnectionManager;
    }

    #region SignalR Events
    public override async Task OnConnectedAsync()
    {
        try
        {
            //  Assuming the driver app sends a unique identifier (e.g., vehicleId or driverid) when connecting
            var key = Context.GetHttpContext().Request.Query["customerId"].ToString();
            _customerConnectionManager.AddConnection(key, Context.ConnectionId);
            _logHelper.LogDebug("Customer Client connected");
            Console.WriteLine($"Connected to customer " + key);

            await base.OnConnectedAsync();
        }
        catch (Exception ex)
        {
            _logHelper.LogError(ex, ex?.Message);
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        try
        {
            _customerConnectionManager.RemoveConnection(Context.ConnectionId);
            _logHelper.LogDebug("Customer Client disconnected");

            await base.OnDisconnectedAsync(exception);
        }
        catch (Exception ex)
        {
            _logHelper.LogError(ex, ex?.Message);
        }
    }
    #endregion

    #region SignalR Server Methods






    #endregion

    #region Private Methods

    #endregion
}
