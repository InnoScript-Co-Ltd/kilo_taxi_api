using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;

namespace KiloTaxi.Realtime.HubInterfaces
{
    public interface IDriverHubHub
    {
        Task SendVehicleLocation(VehicleLocation vehicleLocation);
        
        Task SendVehicleLocationToCustomer(VehicleLocation vehicleLocation,string CustomerId);

        Task SendSos(SosDTO sosDto);
        Task SendTripLocation(TripLocation tripLocation);
        Task SendDriverAvalilityStatus(string AvailityStatus);
        Task SendTripBegin(OrderInfoDTO orderDto);
        Task AcceptOrder(OrderInfoDTO orderDTO);
        Task ArrivedLocation(OrderInfoDTO orderDTO);
        Task SendTripFinish(OrderFormDTO orderDTO,List<OrderExtraDemandDTO> orderExtraDemandDtos);
    }
}
