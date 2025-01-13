using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using KiloTaxi.API.Helper.Authentication.Interface;
using KiloTaxi.Common.ConfigurationSettings;
using KiloTaxi.Common.Enums;
using KiloTaxi.Converter;
using KiloTaxi.DataAccess.Implementation;
using KiloTaxi.EntityFramework;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace KiloTaxi.API.Helper.Authentication.Implementation;

public class AuthenticationService : IAuthenticationService
{
    public readonly AdminRepository _adminRepository;
    public readonly DbKiloTaxiContext _dbContext;
    public readonly IConfiguration _configuration;
    public readonly CustomerRepository _customerRepository;
    public readonly DriverRepository _driverRepository;
    private string _mediaHostUrl;

    public AuthenticationService(
        AdminRepository adminRepository,
        DbKiloTaxiContext dbContext,
        IConfiguration configuration,
        CustomerRepository customerRepository,
        DriverRepository driverRepository,
        IOptions<MediaSettings> mediaSettings
    )
    {
        _adminRepository = adminRepository;
        _dbContext = dbContext;
        _configuration = configuration;
        _customerRepository = customerRepository;
        _driverRepository = driverRepository;
        _mediaHostUrl = mediaSettings.Value.MediaHostUrl;
    }

    public async Task<(string, string)> AuthenticateAdminAsync(string EmailOrPhone, string password)
    {
        var ValidUser = await _adminRepository.ValidateAdminCredentials(EmailOrPhone, password);

        if (ValidUser == null)
        {
            return (null, null);
        }

        var accessToken = GenerateJwtToken(ValidUser.Email, ValidUser.Role);
        var refreshToken = GenerateRefreshToken();
        ValidUser.RefreshToken = refreshToken;
        ValidUser.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
        _adminRepository.UpdateAdmin(
            new AdminFormDTO()
            {
                RefreshToken = refreshToken,
                RefreshTokenExpiryTime = DateTime.Now.AddDays(7),
            }
        );
        return (accessToken, refreshToken);
    }

    public async Task<(string, string, int)> AuthenticateCustomerAsync(
        string EmailOrPhone,
        string password
    )
    {
        var ValidUser = await _customerRepository.ValidateCustomerCredentials(
            EmailOrPhone,
            password
        );

        if (ValidUser == null)
        {
            return (null, null, 0);
        }
        var accessToken = GenerateJwtToken(ValidUser.Phone, ValidUser.Role);
        var refreshToken = GenerateRefreshToken();
        _customerRepository.UpdateCustomer(
            new CustomerFormDTO()
            {
                RefreshToken = refreshToken,
                RefreshTokenExpiryTime = DateTime.Now.AddDays(7),
            }
        );
        return (accessToken, refreshToken, ValidUser.Id);
    }

    public async Task<(string, string, int)> AuthenticateDriverAsync(
        string EmailOrPhone,
        string password
    )
    {
        var ValidUser = await _driverRepository.ValidateDriverCredentials(EmailOrPhone, password);

        if (ValidUser == null)
        {
            return (null, null, 0);
        }
        var accessToken = GenerateJwtToken(ValidUser.Phone, ValidUser.Role);
        var refreshToken = GenerateRefreshToken();

        _driverRepository.UpdateDriver(
            new DriverUpdateFormDTO()
            {
                RefreshToken = refreshToken,
                RefreshTokenExpiryTime = DateTime.Now.AddDays(7),
            }
        );
        return (accessToken, refreshToken, ValidUser.Id);
    }

    // public bool VarifiedOpt(string token,string otp)
    // {
    //     var principal = ValidateToken(token);
    //     var email = principal.FindFirstValue(ClaimTypes.Name);
    //     var role = principal.FindFirstValue(ClaimTypes.Role);
    //     switch (role)
    // {
    //     case "Admin":
    //     Admin admin = _dbContext.Admins.SingleOrDefault(x => x.Email == email);
    //     if (admin.Otp!=otp)
    //     {
    //         return false;
    //     }
    //     var adminDto=AdminConverter.ConvertEntityToModel(admin);
    //     adminDto.Status =CustomerStatus.Active ;
    //     _adminRepository.UpdateAdmin(adminDto);
    //     return true;
    //     break;
    //
    //     case "Customer":
    //     Customer customer = _dbContext.Customers.SingleOrDefault(x => x.Phone == email);
    //     if (customer.Otp!=otp)
    //     {
    //         return false;
    //     }
    //     _customerRepository.UpdateCustomer(new CustomerFormDTO()
    //     {
    //         Status = CustomerStatus.Active ,
    //     });
    //     return true;
    //     break;
    //
    //     case "Driver":
    //     Driver driver = _dbContext.Drivers.SingleOrDefault(x => x.Phone == email);
    //     if (driver.Otp!=otp)
    //     {
    //         return false;
    //     }
    //     _driverRepository.UpdateDriver(new DriverUpdateFormDTO()
    //     {
    //         Status = DriverStatus.Active ,
    //     });
    //     return true;
    //     break;
    // }
    //     return false;
    // }


    public string GenerateJwtToken(string email, string role)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]));
        var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, email),
            new Claim(ClaimTypes.Role, role),
            new Claim(ClaimTypes.Name, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(double.Parse(jwtSettings["ExpirationInMinutes"])),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)); // Secure random string
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]);

        try
        {
            var principal = tokenHandler.ValidateToken(
                token,
                new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                },
                out _
            );

            return principal;
        }
        catch
        {
            return null; // Invalid token
        }
    }

    public (string, string) NewRefreshToken(string email, string role, RefreshTokenDTO request)
    {
        dynamic newAccessToken = null;
        dynamic newRefreshToken = null;
        switch (role)
        {
            case "Admin":
                Admin admin = _dbContext.Admins.SingleOrDefault(x => x.Email == email);
                if (
                    admin == null
                    || admin.RefreshToken != request.RefreshToken
                    || admin.RefreshTokenExpiryTime < DateTime.UtcNow
                )
                {
                    return (null, null);
                }
                newAccessToken = GenerateJwtToken(admin.Email, admin.Role);
                newRefreshToken = GenerateRefreshToken();
                admin.RefreshToken = newRefreshToken;
                admin.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                var adminDto = AdminConverter.ConvertEntityToModel(admin);
                _adminRepository.UpdateAdmin(
                    new AdminFormDTO()
                    {
                        RefreshToken = newRefreshToken,
                        RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7),
                    }
                );
                break;

            case "Customer":
                Customer customer = _dbContext.Customers.SingleOrDefault(x => x.Phone == email);
                if (
                    customer == null
                    || customer.RefreshToken != request.RefreshToken
                    || customer.RefreshTokenExpiryTime < DateTime.UtcNow
                )
                {
                    return (null, null);
                }
                newAccessToken = GenerateJwtToken(customer.Email, customer.Role);
                newRefreshToken = GenerateRefreshToken();
                _customerRepository.UpdateCustomer(
                    new CustomerFormDTO()
                    {
                        RefreshToken = newRefreshToken,
                        RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7),
                    }
                );
                break;

            case "Driver":
                Driver driver = _dbContext.Drivers.SingleOrDefault(x => x.Phone == email);
                if (
                    driver == null
                    || driver.RefreshToken != request.RefreshToken
                    || driver.RefreshTokenExpiryTime < DateTime.UtcNow
                )
                {
                    return (null, null);
                }
                newAccessToken = GenerateJwtToken(driver.Email, driver.Role);
                newRefreshToken = GenerateRefreshToken();
                driver.RefreshToken = newRefreshToken;
                driver.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                _driverRepository.UpdateDriver(
                    new DriverUpdateFormDTO()
                    {
                        RefreshToken = newRefreshToken,
                        RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7),
                    }
                );
                break;
        }
        return (newAccessToken, newRefreshToken);
    }

    public string GenerateOtp()
    {
        return _customerRepository.GenerateOTP();
    }
}
