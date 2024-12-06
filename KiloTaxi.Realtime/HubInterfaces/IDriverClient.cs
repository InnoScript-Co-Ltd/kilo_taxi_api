using KiloTaxi.Model.DTO;

namespace KiloTaxi.Realtime.HubInterfaces
{
    public interface IDriverClient
    {
        Task RequestVehicleLocation(string vehicleId);
        Task RequestSos(SosDTO sosDto);
    }
}
