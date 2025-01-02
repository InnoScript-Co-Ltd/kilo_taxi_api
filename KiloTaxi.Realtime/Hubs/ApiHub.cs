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
    #endregion

    #region Private Methods

    #endregion

}