using System.Security.Claims;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Model.DTO;

namespace KiloTaxi.API.Helper.Authentication.Interface;

public interface IAuthenticationService
{
   
   Task<(string,string)> AuthenticateAdminAsync(string EmailOrPhone, string password);
   
   string GenerateJwtToken(string email,string role);

   
   (string,string) NewRefreshToken(string email,string role,RefreshTokenDTO request);
   string GenerateRefreshToken();
   Task<(string,string,int)> AuthenticateCustomerAsync(string EmailOrPhone,string password);
   
   Task<(string,string,int)> AuthenticateDriverAsync(string EmailOrPhone, string password);
   // bool VarifiedOpt(string token,string otp);
   ClaimsPrincipal ValidateToken(string token);

   string GenerateOtp();
}