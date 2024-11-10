using KiloTaxi.Model.DTO;

namespace KiloTaxi.Realtime.HubInterfaces
{
    public interface IDriverHubHub
    {
        Task SendVehicleLocation(VehicleLocation vehicleLocation);
    }
}
