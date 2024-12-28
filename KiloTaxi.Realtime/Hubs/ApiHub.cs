using KiloTaxi.Logging;
using KiloTaxi.Realtime.HubInterfaces;
using KiloTaxi.Realtime.Services;
using Microsoft.AspNetCore.SignalR;

namespace KiloTaxi.Realtime.Hubs;

public class ApiHub : Hub<IApiClient>, IApiHub
{
    private readonly IHubContext<DriverHub, IDriverClient> _hubDriver;
    private DriverConnectionManager _driverConnectionManager;

    LoggerHelper _logHelper;
    public ApiHub(IHubContext<DriverHub,IDriverClient> hubDriver,DriverConnectionManager driverConnectionManager)
    {
        _logHelper = LoggerHelper.Instance;
        _hubDriver = hubDriver;
        _driverConnectionManager = driverConnectionManager;
    }

    #region SignalR Events
    public async Task SendMessage(string message)
    {
        try
        {
            _logHelper.LogDebug(message);
            Console.WriteLine(message);
            var driverConnectionId = _driverConnectionManager.GetConnectionId("123");

            await _hubDriver.Clients.Client(driverConnectionId).ReceiveTestMethod("brocast to client: "+message);

        }
        catch (Exception ex)
        {
            _logHelper.LogError(ex, ex?.Message);
        }
    }
    #endregion

    #region Private Methods

    #endregion

}