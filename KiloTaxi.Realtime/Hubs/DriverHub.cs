using KiloTaxi.Common.Enums;
using KiloTaxi.DataAccess.Interface;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
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
        private readonly IOrderRepository _orderRepository; // **ADDED: To update order details**
        private readonly IPromotionRepository _promotionRepository;

        public DriverHub(
            DriverConnectionManager driverConnectionManager,
            IHubContext<DashboardHub, IDashboardClient> hubDashboard,
            IHttpClientFactory httpClientFactory,
            IHubContext<ApiHub, IApiClient> hubApi,
            IOrderRepository orderRepository, // **ADDED: Injecting the order repository**
            IPromotionRepository promotionRepository
        )
        {
            _logHelper = LoggerHelper.Instance;
            _driverConnectionManager = driverConnectionManager;
            _hubDashboard = hubDashboard;
            _httpClient = httpClientFactory.CreateClient();
            _hubApi = hubApi;
            _orderRepository = orderRepository; // **Initialize the repository**
            _promotionRepository = promotionRepository;
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

        public async Task SendTripDetails(
            string pickUpLocation,
            string destinationLocation,
            string promoCodeKey,
            List<ExtraDemandDTO> extraDemands
        )
        {
            try
            {
                // **ADDED: Log the trip completion details**
                _logHelper.LogDebug(
                    $"CompleteTrip called with PickUpLocation: {pickUpLocation}, DestinationLocation: {destinationLocation}, PromoCodeKey: {promoCodeKey}"
                );

                // **ADDED: Retrieve and update order details**
                var orderId = Context.GetHttpContext().Request.Query["orderId"].ToString();
                var order = _orderRepository.GetOrderById(int.Parse(orderId));

                if (order == null)
                {
                    _logHelper.LogError($"Order with ID {orderId} not found.");
                    return;
                }

                // Update order details
                order.PickUpLocation = pickUpLocation;
                order.DestinationLocation = destinationLocation;
                order.TotalAmount = CalculateEstimatedAmount(order, extraDemands, promoCodeKey); // Assume this method exists
                order.Status = OrderStatus.Completed;

                if (!_orderRepository.UpdateOrder(order))
                {
                    _logHelper.LogError($"Failed to update order with ID {orderId}.");
                    return;
                }

                _logHelper.LogDebug($"Order with ID {orderId} successfully updated.");

                await _hubApi.Clients.All.ReceiveTripComplete(order, extraDemands);
            }
            catch (Exception ex)
            {
                _logHelper.LogError(ex, ex?.Message);
            }
        }

        #endregion

        #region Private Methods
        private decimal CalculateEstimatedAmount(
            OrderDTO order,
            List<ExtraDemandDTO> extraDemands,
            string promoCodeKey
        )
        {
            decimal baseAmount = order.EstimatedAmount ?? 0m;

            decimal extraCost = extraDemands.Sum(demand => demand.Amount);

            // Fetch the discount from the promo code (if any)
            decimal discount = GetPromoDiscount(promoCodeKey);

            decimal estimatedAmount = baseAmount + extraCost - discount;

            return Math.Max(0, estimatedAmount);
        }

        private decimal GetPromoDiscount(string promoCodeKey)
        {
            try
            {
                // Fetch promotion using the promo code
                var promotion = _promotionRepository
                    .GetAllPromotion(
                        new PageSortParam
                        { /* Pagination logic if necessary */
                        }
                    )
                    .Promotions.FirstOrDefault(p =>
                        p.PromoCode.Equals(promoCodeKey, StringComparison.OrdinalIgnoreCase)
                        && p.ExpiredDate >= DateTime.UtcNow
                        && p.Status == PromotionStatus.Active
                    );

                if (promotion == null)
                {
                    return 0m; // No valid promotion
                }

                // Depending on the PromotionType, calculate the discount
                if (promotion.PromotionType == PromotionType.FixAmount)
                {
                    return promotion.Unit; // Fixed discount (e.g., $50)
                }
                else if (promotion.PromotionType == PromotionType.Percentage)
                {
                    // If the promotion is percentage-based, calculate discount based on the order amount
                    return promotion.Unit; // In this case, Unit would be the percentage value (e.g., 10 for 10% off)
                }

                return 0m; // Default if no valid promo type
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(ex, "Error occurred while applying promotion.");
                return 0m; // Return no discount in case of error
            }
        }

        #endregion
    }
}
