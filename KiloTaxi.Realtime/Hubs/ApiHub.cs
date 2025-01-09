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
    private DriverConnectionManager _driverConnectionManager;
    private readonly IHubContext<CustomerHub, ICustomerClient> _hubCustomer;

    LoggerHelper _logHelper;

    public ApiHub(IHubContext<DriverHub,IDriverClient> hubDriver,DriverConnectionManager driverConnectionManager,
                    IHubContext<CustomerHub, ICustomerClient> hubCustomer)
    {
        _logHelper = LoggerHelper.Instance;
        _hubDriver = hubDriver;
        _hubCustomer = hubCustomer;
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
        await _hubCustomer.Clients.Client(orderDTO.CustomerId.ToString()).ReceiveDriverInfo(orderDTO, driverDTO);
    }


    #region Private Methods

    #endregion
}
