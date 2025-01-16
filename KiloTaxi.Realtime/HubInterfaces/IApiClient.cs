using KiloTaxi.Common.Enums;
using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;

namespace KiloTaxi.Realtime.HubInterfaces;

public interface IApiClient
{
    Task ReceiveAvailityStatus(string availabilityStatus, int key);
    Task ReceiveTripLocation(TripLocation tripLocation);

    Task AcceptOrderAsync(OrderInfoDTO orderDTO, int driverID);
    Task ReceiveOrderUpdate(int orderId, OrderStatus orderStatus);
    
    Task ArrivedLocation(OrderInfoDTO orderDTO, int driverID);
    
    Task TripBegin(OrderInfoDTO orderDTO);
    
    Task SendFinishOrder(OrderFormDTO orderDTO,List<OrderExtraDemandDTO> orderExtraDemandDTOs);
}
