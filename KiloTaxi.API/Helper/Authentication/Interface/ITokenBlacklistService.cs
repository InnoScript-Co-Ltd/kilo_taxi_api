namespace KiloTaxi.API.Helper.Authentication.Interface;

public interface  ITokenBlacklistService
{
    Task AddTokenToBlacklistAsync(string token, DateTime expiryTime);
    Task<bool> IsTokenBlacklistedAsync(string token);
}