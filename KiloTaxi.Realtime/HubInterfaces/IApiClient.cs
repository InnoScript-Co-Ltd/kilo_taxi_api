using KiloTaxi.Model.DTO;

namespace KiloTaxi.Realtime.HubInterfaces;

public interface IApiClient
{
    Task ReceiveAvailityStatus(string availabilityStatus, int key);
    Task ReceiveTripLocation(TripLocation tripLocation);

    Task AcceptOrderAsync(OrderDTO orderDTO, int driverID);
    
    Task ArrivedLocation(OrderDTO orderDTO, int driverID);
    
    Task TripBegin(OrderDTO orderDTO);
}
