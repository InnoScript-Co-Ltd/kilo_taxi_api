using KiloTaxi.DataAccess.Interface;
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
        private readonly HttpClient _httpClient;
        private readonly IHubContext<ApiHub, IApiClient> _hubApi;

        public DriverHub(DriverConnectionManager driverConnectionManager, IHubContext<DashboardHub,IDashboardClient> hubDashboard,IHttpClientFactory httpClientFactory,IHubContext<ApiHub, IApiClient> hubApi)
        {
            _logHelper = LoggerHelper.Instance;
            _driverConnectionManager = driverConnectionManager;
            _hubDashboard = hubDashboard;
            _httpClient = httpClientFactory.CreateClient();
            _hubApi = hubApi;

        }

        #region SignalR Events
        public override async Task OnConnectedAsync()
        {
            try
            {                
                // Assuming the driver app sends a unique identifier (e.g., vehicleId or driverId) when connecting
                var key = Context.GetHttpContext().Request.Query["driverId"].ToString();
                _driverConnectionManager.AddConnection(key, Context.ConnectionId);
                Console.WriteLine($"Connected to"+key);
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
                var response = await _httpClient.PostAsJsonAsync("https://localhost:7181/api/v1/Sos", sosDto);

                if (!response.IsSuccessStatusCode)
                {
                    _logHelper.LogError($"Failed to create SOS: {response.StatusCode} {response.ReasonPhrase}");
                }

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
