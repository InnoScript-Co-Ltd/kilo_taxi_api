using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Response;

namespace KiloTaxi.Realtime.HubInterfaces;

public interface IApiHub
{
    Task SendOrder(OrderDTO orderDTO, List<DriverInfoDTO> driverInfoDTOs);

    Task ReceiveTripComplete(OrderDTO order, List<ExtraDemandDTO> extraDemands);

    Task SendTripCompleteToCustomer(
        string pickupLocation,
        string destinationLocation,
        decimal actualTotalPrice,
        decimal promoCodeDiscount,
        List<ExtraDemandDTO> extraDemands
    );
}
