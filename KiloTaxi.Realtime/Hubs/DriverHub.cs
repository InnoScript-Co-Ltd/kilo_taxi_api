using KiloTaxi.Common.Enums;
using KiloTaxi.DataAccess.Interface;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;
using KiloTaxi.Realtime.HubInterfaces;
using KiloTaxi.Realtime.Services;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.SignalR;

namespace KiloTaxi.Realtime.Hubs
{
    public class DriverHub : Hub<IDriverClient>, IDriverHubHub
    {
        LoggerHelper _logHelper;
        private DriverConnectionManager _driverConnectionManager;
        private DashBoardConnectionManager _dashBoardConnectionManager;
        private readonly IHubContext<DashboardHub, IDashboardClient> _hubDashboard;
        private readonly HttpClient _httpClient;
        private readonly IHubContext<ApiHub, IApiClient> _hubApi;

        public DriverHub(
            DriverConnectionManager driverConnectionManager,
            IHubContext<DashboardHub, IDashboardClient> hubDashboard,
            IHttpClientFactory httpClientFactory,
            IHubContext<ApiHub, IApiClient> hubApi
        )
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
                // Assuming the driver app sends a unique identifier (e.g., vehicleId or driverid) when connecting
                var key = Context.GetHttpContext().Request.Query["driverId"].ToString();
                _driverConnectionManager.AddConnection(key, Context.ConnectionId);
                Console.WriteLine($"Connected to" + key);
                _logHelper.LogDebug("Driver Client connected");

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
                var response = await _httpClient.PostAsJsonAsync(
                    "https://localhost:7181/api/v1/Sos",
                    sosDto
                );

                if (!response.IsSuccessStatusCode)
                {
                    _logHelper.LogError(
                        $"Failed to create SOS: {response.StatusCode} {response.ReasonPhrase}"
                    );
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

        public async Task SendTripLocation(TripLocation tripLocation)
        {
            try
            {
                _logHelper.LogDebug(
                    $"SendTripLocation: OrderId = {tripLocation.OrderId}, Latitude = {tripLocation.Lat}, Longitude = {tripLocation.Long}"
                );

                // Broadcast to the ApiClientHub
                await _hubApi.Clients.All.ReceiveTripLocation(tripLocation);
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex, ex.Message);
            }
        }

        public async Task SendDriverAvalilityStatus(string AvailityStatus)
        {
            try
            {
                var key = Context.GetHttpContext().Request.Query["driverId"].ToString();

                await _hubApi.Clients.All.ReceiveAvailityStatus(AvailityStatus, int.Parse(key));
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex, ex?.Message);
            }
        }

        public async Task SendOrderCompleteStatus(int orderId, List<ExtraDemandDTO> extraDemands)
        {
            try
            {
                var orderStatus = OrderStatus.Completed;

                await _hubApi.Clients.All.ReceiveOrderUpdate(orderId, orderStatus);

                _logHelper.LogDebug($"Order complete status sent for Order ID {orderId}.");
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex, "Error occurred while sending order complete status.");
            }
        }

        public async Task AcceptOrder(OrderInfoDTO orderDTO)
        {
            try
            {
                var key = Context.GetHttpContext().Request.Query["driverId"].ToString();
                var driverConnectionId = _driverConnectionManager.GetConnectionId(key);

                _driverConnectionManager.NotifyOrderAccepted(driverConnectionId, orderDTO.Id.ToString());
                Console.WriteLine($"Driver {key} accepted order {orderDTO.Id}.");

                await _hubApi.Clients.All.AcceptOrderAsync(orderDTO, int.Parse(key));
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex, ex?.Message);
            }
        }
        public async Task ArrivedLocation(OrderInfoDTO orderDTO)
        {
            try
            {
                var key = Context.GetHttpContext().Request.Query["driverId"].ToString();
                await _hubApi.Clients.All.ArrivedLocation(orderDTO, int.Parse(key));
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex, ex?.Message);
            }

        }
        public async Task SendTripBegin(OrderInfoDTO orderDTO)
        {
            try
            {
                await _hubApi.Clients.All.TripBegin(orderDTO);
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex, ex?.Message);
            }

        }
        
        public async Task SendTripFinish(OrderFormDTO orderDTO,List<OrderExtraDemandDTO> orderExtraDemands)
        {
            try
            {
                Console.WriteLine("SendTripFinish");
                
                await _hubApi.Clients.All.SendFinishOrder(orderDTO,orderExtraDemands);
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
