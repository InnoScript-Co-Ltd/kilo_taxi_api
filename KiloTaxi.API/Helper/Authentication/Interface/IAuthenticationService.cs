namespace KiloTaxi.API.Helper.Authentication.Interface;

public interface IAuthenticationService
{
   
   Task<string> AuthenticateAdminAsync(string username, string password);
   
   string GenerateJwtToken(string username);
}