using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using KiloTaxi.API.Helper.Authentication.Interface;
using KiloTaxi.DataAccess.Implementation;
using KiloTaxi.EntityFramework;
using Microsoft.IdentityModel.Tokens;

namespace KiloTaxi.API.Helper.Authentication.Implementation;

public class AuthenticationService : IAuthenticationService
{
    public readonly AdminRepository _adminRepository;
    public readonly DbKiloTaxiContext _dbContext;
    public readonly IConfiguration _configuration;
    public AuthenticationService(AdminRepository adminRepository,DbKiloTaxiContext dbContext,IConfiguration configuration)
    {
        _adminRepository = adminRepository;
        _dbContext = dbContext;
        _configuration = configuration;
    }
    

    public async Task<string> AuthenticateAdminAsync(string email, string password)
    {
        var isValidUser = await _adminRepository.ValidateAdminCredentials(email, password);

        if (!isValidUser)
        {
            return null;
        }            


        return GenerateJwtToken(email);    
    }

    public string GenerateJwtToken(string username)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]));
        var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(double.Parse(jwtSettings["ExpirationInMinutes"])),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);

    }
}