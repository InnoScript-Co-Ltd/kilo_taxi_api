using System.Security.Claims;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Model.DTO;

namespace KiloTaxi.API.Helper.Authentication.Interface;

public interface IAuthenticationService
{
   
   Task<(string,string,string)> AuthenticateAdminAsync(string EmailOrPhone, string password);
   
   string GenerateJwtToken(string username,string role);

   
   (string,string) NewRefreshToken(string email,string role,RefreshTokenDTO request);
   string GenerateRefreshToken();
   Task<(string,string,string)> AuthenticateCustomerAsync(string EmailOrPhone,string password);
   
   Task<(string,string,string)> AuthenticateDriverAsync(string EmailOrPhone, string password);
   bool VarifiedOpt(string token,string otp);
   ClaimsPrincipal ValidateToken(string token);
}