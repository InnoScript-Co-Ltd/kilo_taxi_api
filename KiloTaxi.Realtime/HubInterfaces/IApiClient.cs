namespace KiloTaxi.Realtime.HubInterfaces;

public interface IApiClient
{
    Task ReceiveAvailityStatus(string availabilityStatus,int key);
}