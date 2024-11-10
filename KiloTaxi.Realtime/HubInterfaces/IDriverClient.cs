namespace KiloTaxi.Realtime.HubInterfaces
{
    public interface IDriverClient
    {
        Task RequestVehicleLocation(string vehicleId);
    }
}
