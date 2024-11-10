using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using KiloTaxi.Realtime.HubInterfaces;
using KiloTaxi.Realtime.Services;
using Microsoft.AspNetCore.SignalR;

namespace KiloTaxi.Realtime.Hubs
{
    public class DashboardHub : Hub<IDashboardClient>, IDashboardHub
    {
        LoggerHelper _logHelper;
        private DriverConnectionManager _driverConnectionManager;
        private readonly IHubContext<DriverHub, IDriverClient> _hubDriver;
        public DashboardHub(DriverConnectionManager driverConnectionManager, IHubContext<DriverHub,IDriverClient> hubDriver)
        {
            _logHelper = LoggerHelper.Instance;
            _driverConnectionManager = driverConnectionManager;
            _hubDriver = hubDriver;
        }

        #region SignalR Events
        public override Task OnConnectedAsync()
        {
            try
            {
                //_clientConnectionService.AddClient(Context.ConnectionId);                

                _logHelper.LogDebug("Dashbaord Client connected");

            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex, ex?.Message);
            }
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                //_clientConnectionService.RemoveClient(Context.ConnectionId);
                _logHelper.LogDebug("Dashbaord Client disconnected");

            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex, ex?.Message);
            }
            return base.OnDisconnectedAsync(exception);
        }
        #endregion

        #region SignalR Server Methods
        public async Task RequestVehicleLocation(string vehicleId)
        {
            try
            {
                var driverConnectionId = _driverConnectionManager.GetConnectionId(vehicleId);

                if (driverConnectionId != null)
                {
                    await _hubDriver.Clients.Client(driverConnectionId).RequestVehicleLocation(vehicleId);

                }
                else
                {
                    // Optionally, handle case where mobile client is not connected
                    _logHelper.LogDebug("Vehicle not connected: " + vehicleId);
                }
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
}
