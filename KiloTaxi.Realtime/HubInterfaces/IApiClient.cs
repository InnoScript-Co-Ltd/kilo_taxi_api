namespace KiloTaxi.Realtime.HubInterfaces;

public interface IApiClient
{
    Task ReceiveTestMethod(string data);
}