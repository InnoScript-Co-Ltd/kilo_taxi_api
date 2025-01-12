using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Response;

namespace KiloTaxi.Realtime.HubInterfaces;

public interface IApiHub
{
    Task SendOrder(OrderDTO orderDTO, List<DriverInfoDTO> driverInfoDTOs);
    
    Task SendDriverInfoToCustomer(OrderDTO order, DriverInfoDTO driverDTO);
    
    Task SendReceiveDriverArrivedLocation(OrderDTO order, DriverInfoDTO driverInfoDTO);
    
    Task SendTripBeginToCustomer(OrderDTO order);
}
