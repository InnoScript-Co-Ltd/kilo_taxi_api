namespace KiloTaxi.Realtime.HubInterfaces;

public interface IApiHub
{
    Task SendMessage(string data);
}