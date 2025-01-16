using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;

namespace KiloTaxi.Realtime.HubInterfaces;

public interface ICustomerClient
{
    Task ReceiveDriverInfo(OrderDTO orderDTO, DriverInfoDTO driverDTO);
    Task  ReceiveTripBegin(OrderDTO orderDTO);
    Task ReceiveDriverArrivedLocation(OrderDTO orderDTO, DriverInfoDTO driverInfoDTO);

    Task ReceiveTripComplete(
        OrderFormDTO orderDTO, PromotionUsageDTO promotionUsageDTO,
        List<OrderExtraDemandDTO> orderExtraDemandDtos
    );
}
