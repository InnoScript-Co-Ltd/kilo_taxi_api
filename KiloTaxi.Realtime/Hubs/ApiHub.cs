using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Response;
using KiloTaxi.Realtime.HubInterfaces;
using KiloTaxi.Realtime.Services;
using Microsoft.AspNetCore.SignalR;

namespace KiloTaxi.Realtime.Hubs;

public class ApiHub : Hub<IApiClient>, IApiHub
{
    private readonly IHubContext<DriverHub, IDriverClient> _hubDriver;
    private DriverConnectionManager _driverConnectionManager;

    LoggerHelper _logHelper;

    public ApiHub(
        IHubContext<DriverHub, IDriverClient> hubDriver,
        DriverConnectionManager driverConnectionManager
    )
    {
        _logHelper = LoggerHelper.Instance;
        _hubDriver = hubDriver;
        _driverConnectionManager = driverConnectionManager;
    }

    #region SignalR Events
    #endregion
    public async Task SendOrder(OrderDTO orderDTO, List<DriverInfoDTO> driverInfoDTOs)
    {
        try
        {
            var driverConnectionId = _driverConnectionManager.GetConnectionId(
                driverInfoDTOs[0].Id.ToString()
            );

            if (driverConnectionId != null)
            {
                await _hubDriver.Clients.Client(driverConnectionId).ReceiveOrder(orderDTO);
            }
            else
            {
                // Optionally, handle case where mobile client is not connected
                _logHelper.LogDebug("Vehicle not connected: " + driverConnectionId);
            }
        }
        catch (Exception ex)
        {
            _logHelper.LogError(ex, ex?.Message);
        }
    }

    #region Private Methods

    #endregion
}
