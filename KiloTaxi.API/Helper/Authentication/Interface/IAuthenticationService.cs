using System.Security.Claims;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Model.DTO;

namespace KiloTaxi.API.Helper.Authentication.Interface;

public interface IAuthenticationService
{
   
   Task<(string,string)> AuthenticateAdminAsync(string username, string password);
   
   string GenerateJwtToken(string username,string role);
   (string,string) NewRefreshToken(string email,string role,RefreshTokenDTO request);
   string GenerateRefreshToken();
   Task<(string,string)> AuthenticateCustomerAsync(string username, string password);
   
   Task<(string,string)> AuthenticateDriverAsync(string username, string password);

   ClaimsPrincipal ValidateToken(string token);
}