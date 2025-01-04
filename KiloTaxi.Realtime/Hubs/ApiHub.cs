using KiloTaxi.DataAccess.Interface;
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
    private readonly IHubContext<CustomerHub, ICustomerClient> _hubCustomer; // Assuming you have a CustomerHub for handling customer clients
    private DriverConnectionManager _driverConnectionManager;
    private readonly CustomerConnectionManager _customerConnectionManager;

    private readonly IOrderRepository _orderService; // Service to handle order updates
    private readonly ITransactionLogRepository _transactionService; // Service to handle transaction logs

    LoggerHelper _logHelper;

    public ApiHub(
        IHubContext<DriverHub, IDriverClient> hubDriver,
        IHubContext<CustomerHub, ICustomerClient> hubCustomer, // Injected IHubContext for customer
        DriverConnectionManager driverConnectionManager,
        CustomerConnectionManager customerConnectionManager,
        IOrderRepository orderService,
        ITransactionLogRepository transactionService
    )
    {
        _logHelper = LoggerHelper.Instance;
        _hubDriver = hubDriver;
        _hubCustomer = hubCustomer; // Initialize the customer hub context
        _driverConnectionManager = driverConnectionManager;
        _customerConnectionManager = customerConnectionManager;
        _orderService = orderService; // Initialize the order service
        _transactionService = transactionService; // Initialize the transaction service
    }

    #region SignalR Events
    public override Task OnConnectedAsync()
    {
        var customerId = Context.GetHttpContext()?.Request.Query["customerId"];
        if (!string.IsNullOrEmpty(customerId))
        {
            _customerConnectionManager.AddConnection(customerId, Context.ConnectionId);
        }
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        _customerConnectionManager.RemoveConnection(Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }

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

    public async Task ReceiveTripComplete(OrderDTO order, List<ExtraDemandDTO> extraDemands)
    {
        try
        {
            _logHelper.LogDebug(
                $"Trip complete for OrderId: {order.Id}, extra demands: {extraDemands.Count}"
            );

            decimal actualTotalPrice = order.TotalAmount;

            decimal promoCodeDiscount = 0;

            // Calculate final price after applying the promo code discount
            decimal finalAmount = actualTotalPrice - promoCodeDiscount;

            // Send the trip completion notification to the customer
            var customerConnectionId = _customerConnectionManager.GetConnectionId(
                order.CustomerId.ToString()
            );

            if (!string.IsNullOrEmpty(customerConnectionId))
            {
                await _hubCustomer
                    .Clients.Client(customerConnectionId)
                    .ReceiveTripComplete(
                        order.PickUpLocation,
                        order.DestinationLocation,
                        finalAmount, // Final price after applying the discount
                        promoCodeDiscount, // The discount value
                        extraDemands
                    );
            }
            else
            {
                _logHelper.LogDebug($"Customer with ID {order.CustomerId} is not connected.");
            }

            // Optional: Log the transaction for trip completion
            // var transaction = new TransactionLog
            // {
            //     OrderId = order.Id,
            //     Action = "Trip completed",
            //     Amount = finalAmount,
            //     PromoCodeDiscount = promoCodeDiscount,
            // };

            // await _transactionService.AddTransactionLog(transaction);
        }
        catch (Exception ex)
        {
            _logHelper.LogError(
                ex,
                $"Error occurred while processing trip completion for OrderId: {order.Id}"
            );
        }
    }

    public async Task SendTripCompleteToCustomer(
        string pickupLocation,
        string destinationLocation,
        decimal actualTotalPrice,
        decimal promoCodeDiscount,
        List<ExtraDemandDTO> extraDemands
    )
    {
        try
        {
            await _hubCustomer.Clients.All.ReceiveTripComplete(
                pickupLocation,
                destinationLocation,
                actualTotalPrice,
                promoCodeDiscount,
                extraDemands
            );
        }
        catch (Exception ex)
        {
            _logHelper.LogError(
                ex,
                "Error occurred while sending trip complete information to customer."
            );
        }
    }

    #region Private Methods


    #endregion
}
