using System.Net.Http;
using System.Text.Json;
using KiloTaxi.Common.Enums;
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
    private readonly IHubContext<CustomerHub, ICustomerClient> _hubCustomer;
    private DriverConnectionManager _driverConnectionManager;
    private CustomerConnectionManager _customerConnectionManager;

    private readonly IOrderRepository _orderService;

    LoggerHelper _logHelper;

    public ApiHub(
        IHubContext<DriverHub, IDriverClient> hubDriver,
        DriverConnectionManager driverConnectionManager,
        CustomerConnectionManager customerConnectionManager,
        IOrderRepository orderService, // Injected here
        IHubContext<CustomerHub, ICustomerClient> hubCustomer
    )
    {
        _logHelper = LoggerHelper.Instance;
        _hubDriver = hubDriver;
        _hubCustomer = hubCustomer;
        _driverConnectionManager = driverConnectionManager;
        _customerConnectionManager = customerConnectionManager;
        _orderService = orderService; // Initialize _orderService
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
            Console.WriteLine("send order");

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

    public async Task SendDriverInfoToCustomer(OrderDTO orderDTO, DriverInfoDTO driverDTO)
    {
        Console.WriteLine("API Hub: " + orderDTO.CustomerId);
        if (orderDTO == null)
        {
            throw new ArgumentNullException(nameof(orderDTO), "OrderDTO cannot be null.");
        }

        if (driverDTO == null)
        {
            throw new ArgumentNullException(nameof(driverDTO), "DriverDTO cannot be null.");
        }

        Console.WriteLine("API Hub 1");

        // Send data to SignalR hub
        //await _hubDriver.Clients.All.SendAsync("ReceiveDriverInfo", payload);
        await _hubCustomer
            .Clients.Client(orderDTO.CustomerId.ToString())
            .ReceiveDriverInfo(orderDTO, driverDTO);
    }

    public async Task NotifyCustomerTripComplete(OrderDTO order, List<ExtraDemandDTO> extraDemands)
    {
        try
        {
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
                        order.TotalAmount ?? 0m, // Use 0m as the default value if TotalAmount is null
                        0,
                        extraDemands
                    );
            }
            else
            {
                _logHelper.LogDebug($"Customer with ID {order.CustomerId} is not connected.");
            }
        }
        catch (Exception ex)
        {
            _logHelper.LogError(ex, "Error while notifying customer about trip completion.");
        }
    }

    public async Task UpdateOrderStatus(
        int orderId,
        OrderStatus orderStatus,
        List<ExtraDemandDTO> extraDemands
    )
    {
        try
        {
            // Fetch the order by ID
            var order = _orderService.GetOrderById(orderId);
            if (order == null)
            {
                _logHelper.LogDebug($"Order with ID {orderId} not found.");
                return;
            }

            // Update the order status
            order.Status = orderStatus;
            var isOrderUpdated = _orderService.UpdateOrder(order);

            if (!isOrderUpdated)
            {
                _logHelper.LogError($"Failed to update order status for Order ID {orderId}.");
                return;
            }

            // Optionally handle extra demands (add your logic here)
            foreach (var extraDemand in extraDemands)
            {
                // Update or insert extra demands in the database
            }

            _logHelper.LogDebug($"Order status updated to {orderStatus} for Order ID {orderId}.");

            // Notify clients about the updated order status
            await Clients.All.ReceiveOrderUpdate(orderId, orderStatus);
        }
        catch (Exception ex)
        {
            _logHelper.LogError(ex, "Error occurred while updating order status.");
        }
    }

    #region Private Methods

    #endregion
}
