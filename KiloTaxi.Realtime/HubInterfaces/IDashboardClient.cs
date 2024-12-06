using KiloTaxi.Model.DTO;

namespace KiloTaxi.Realtime.HubInterfaces
{
    public interface IDashboardClient
    {
        Task ReceiveLocationData(VehicleLocation vehicleLocation);
        Task ReceiveSos(SosDTO sosDto);
    }
}
