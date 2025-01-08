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

    LoggerHelper _logHelper;

    public ApiHub(
        IHubContext<DriverHub, IDriverClient> hubDriver,
        DriverConnectionManager driverConnectionManager,
        CustomerConnectionManager customerConnectionManager
    )
    {
        _logHelper = LoggerHelper.Instance;
        _hubDriver = hubDriver;
        _driverConnectionManager = driverConnectionManager;
        _customerConnectionManager = customerConnectionManager;
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
