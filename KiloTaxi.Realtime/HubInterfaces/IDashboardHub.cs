namespace KiloTaxi.Realtime.HubInterfaces
{
    public interface IDashboardHub
    {
        Task RequestVehicleLocation(string vehicleId);
    }
}
