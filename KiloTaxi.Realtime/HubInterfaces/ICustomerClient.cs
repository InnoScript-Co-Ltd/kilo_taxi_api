using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Response;

namespace KiloTaxi.Realtime.HubInterfaces;

public interface ICustomerClient
{
    Task ReceiveDriverInfo(OrderDTO orderDTO, DriverInfoDTO driverDTO);
}