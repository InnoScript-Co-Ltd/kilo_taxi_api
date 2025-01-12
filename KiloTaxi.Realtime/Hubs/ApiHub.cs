using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Response;
using KiloTaxi.Realtime.HubInterfaces;
using KiloTaxi.Realtime.Services;
using Microsoft.AspNetCore.SignalR;
using System.Net.Http;
using System.Text.Json;

namespace KiloTaxi.Realtime.Hubs;

public class ApiHub : Hub<IApiClient>, IApiHub
{
    private readonly IHubContext<DriverHub, IDriverClient> _hubDriver;
    private readonly IHubContext<CustomerHub, ICustomerClient> _hubCustomer;
    private DriverConnectionManager _driverConnectionManager;
    private CustomerConnectionManager _customerConnectionManager;

    LoggerHelper _logHelper;

    public ApiHub(
        IHubContext<DriverHub, IDriverClient> hubDriver,
        DriverConnectionManager driverConnectionManager,
        CustomerConnectionManager customerConnectionManager,
        IHubContext<CustomerHub, ICustomerClient> hubCustomer)
    {
        _logHelper = LoggerHelper.Instance;
        _hubDriver = hubDriver;
        _hubCustomer = hubCustomer;
        _driverConnectionManager = driverConnectionManager;
        _customerConnectionManager = customerConnectionManager;
    }

    #region SignalR Events
    #endregion
    public async Task SendOrder(OrderDTO orderDTO, List<DriverInfoDTO> driverInfoDTOs)
    {
        try
        {
          foreach (var driverInfo in driverInfoDTOs)
          { 
               var driverConnectionId = _driverConnectionManager.GetConnectionId(driverInfo.Id.ToString());
            
               if (driverConnectionId != null)
               {
                   var orderAccepted = false;
                   var responseReceived = false;
            
                   // Subscribe to driver's response
                   _driverConnectionManager.SubscribeToDriverResponse(driverConnectionId, response =>
                   {
                       orderAccepted = response;
                       responseReceived = true;
                   });
            
                   // Send order to driver
                   await _hubDriver.Clients.Client(driverConnectionId).ReceiveOrder(orderDTO);
            
                   // Wait for a response or timeout
                   var timeoutTask = Task.Delay(TimeSpan.FromSeconds(20));
                   while (!responseReceived && !timeoutTask.IsCompleted)
                   {
                       await Task.Delay(100); // Check periodically
                   }
            
                   // Unsubscribe from the response
                   _driverConnectionManager.UnsubscribeFromDriverResponse(driverConnectionId);
            
                   // If order was accepted, stop the loop
                   if (orderAccepted)
                   {
                       Console.WriteLine($"Order accepted by driver {driverInfo.Id}");
                       return;
                   }
                   else
                   {
                       Console.WriteLine($"Driver {driverInfo.Id} did not accept the order.");
                   }
               }
               else
               {
                   Console.WriteLine($"Driver {driverInfo.Id} is not connected.");
               }
          }
          Console.WriteLine("No drivers accepted the order.");
        }
        catch (Exception ex)
        {
            _logHelper.LogError(ex, ex?.Message);
        }
    }

    public async Task SendDriverInfoToCustomer(OrderDTO orderDTO, DriverInfoDTO driverDTO)
    {
        
        var customerConnectionId = _customerConnectionManager.GetConnectionId(orderDTO.CustomerId.ToString());
        Console.WriteLine("Customer ConnectionId:"+customerConnectionId);
        if (string.IsNullOrEmpty(customerConnectionId))
        {
            Console.WriteLine("Customer ConnectionId is null or empty.");
        }
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
        await _hubCustomer.Clients.Client(customerConnectionId).ReceiveDriverInfo(orderDTO, driverDTO);
    }
    public async Task SendReceiveDriverArrivedLocation(OrderDTO orderDTO, DriverInfoDTO driverDTO)
    {
        var customerConnectionId = _customerConnectionManager.GetConnectionId(orderDTO.CustomerId.ToString());
        Console.WriteLine("Customer ConnectionId:"+customerConnectionId);
        if (string.IsNullOrEmpty(customerConnectionId))
        {
            Console.WriteLine("Customer ConnectionId is null or empty.");
        }
        if (orderDTO == null)
        {
            throw new ArgumentNullException(nameof(orderDTO), "OrderDTO cannot be null.");
        }

        if (driverDTO == null)
        {
            throw new ArgumentNullException(nameof(driverDTO), "DriverDTO cannot be null.");
        }
        await _hubCustomer.Clients.Client(customerConnectionId).ReceiveDriverArrivedLocation(orderDTO, driverDTO);
    }
    
    public async Task SendTripBeginToCustomer(OrderDTO orderDTO)
    {
        var customerConnectionId = _customerConnectionManager.GetConnectionId(orderDTO.CustomerId.ToString());
        Console.WriteLine("Customer ConnectionId:"+customerConnectionId);
        if (string.IsNullOrEmpty(customerConnectionId))
        {
            Console.WriteLine("Customer ConnectionId is null or empty.");
        }
        if (orderDTO == null)
        {
            throw new ArgumentNullException(nameof(orderDTO), "OrderDTO cannot be null.");
        }
        await _hubCustomer.Clients.Client(customerConnectionId).ReceiveTripBegin(orderDTO);
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

    #region Private Methods

    #endregion
}
