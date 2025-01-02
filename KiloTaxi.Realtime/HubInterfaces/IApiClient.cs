using KiloTaxi.Model.DTO;

namespace KiloTaxi.Realtime.HubInterfaces;

public interface IApiClient
{
    Task AcceptOrderAsync(OrderDTO orderDTO, int driverID);
}