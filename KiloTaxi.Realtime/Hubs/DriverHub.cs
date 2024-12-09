using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using KiloTaxi.Realtime.HubInterfaces;
using KiloTaxi.Realtime.Services;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.SignalR;

namespace KiloTaxi.Realtime.Hubs
{
    public class DriverHub:Hub<IDriverClient>,IDriverHubHub
    {
        LoggerHelper _logHelper;
        private DriverConnectionManager _driverConnectionManager;
        private DashBoardConnectionManager _dashBoardConnectionManager;
        private readonly IHubContext<DashboardHub,IDashboardClient> _hubDashboard;
        public DriverHub(DriverConnectionManager driverConnectionManager, IHubContext<DashboardHub,IDashboardClient> hubDashboard)
        {
            _logHelper = LoggerHelper.Instance;
            _driverConnectionManager = driverConnectionManager;
            _hubDashboard = hubDashboard;
        }

        #region SignalR Events
        public override async Task OnConnectedAsync()
        {
            try
            {                
                // Assuming the driver app sends a unique identifier (e.g., vehicleId or driverid) when connecting
                var key = Context.GetHttpContext().Request.Query["vehicleId"].ToString();
                _driverConnectionManager.AddConnection(key, Context.ConnectionId);
                _logHelper.LogDebug("Driver Client connected");

                await base.OnConnectedAsync();
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex, ex?.Message);
            }
            
        }

        public override  async Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                _driverConnectionManager.RemoveConnection(Context.ConnectionId);
                _logHelper.LogDebug("Driver Client disconnected");

                await base.OnDisconnectedAsync(exception);
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex, ex?.Message);
            }            
        }
        #endregion

        #region SignalR Server Methods
       
        
        public async Task SendVehicleLocation(VehicleLocation vehicleLocation)
        {
            try
            {
                // Handle the data received from the client
                _logHelper.LogDebug($"SendVehicleLocation {vehicleLocation.VehicleId}");

                await _hubDashboard.Clients.All.ReceiveLocationData(vehicleLocation);
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex, ex?.Message);
            }
        }
        public async Task SendSos(SosDTO sosDto)
        {
            try
            {
                // Handle the data received from the client
                _logHelper.LogDebug($"SendSos {sosDto}");

                await _hubDashboard.Clients.All.ReceiveSos(sosDto);
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
