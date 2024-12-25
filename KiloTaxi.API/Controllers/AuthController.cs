using System.Security.Claims;
using KiloTaxi.API.Helper.Authentication.Interface;
using KiloTaxi.EntityFramework;
using KiloTaxi.Model.DTO;
using Microsoft.AspNetCore.Mvc;

namespace KiloTaxi.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IConfiguration _configuration;


    public AuthController(IAuthenticationService authenticationService, IConfiguration configuration)
    {
        _authenticationService = authenticationService;
        _configuration = configuration;
    }
    
    [HttpPost("adminLogin")]
    public async Task<IActionResult> Login([FromBody] AuthDTO authDto)
    {
        if (authDto == null || string.IsNullOrEmpty(authDto.Email) || string.IsNullOrEmpty(authDto.Password))
        {
            return BadRequest("Invalid login request.");
        }

        var (accessToken, refreshToken) = await _authenticationService.AuthenticateAdminAsync(authDto.Email, authDto.Password);            
      
        if (string.IsNullOrEmpty(accessToken))
        {
            return Unauthorized("Invalid credentials");
        }

        //return Ok(new { Token = token });
        return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken });
    }
    
    [HttpPost("customerLogin")]
    public async Task<IActionResult> customerLogin([FromBody] AuthDTO authDto)
    {
        if (authDto == null || string.IsNullOrEmpty(authDto.Email) || string.IsNullOrEmpty(authDto.Password))
        {
            return BadRequest("Invalid login request.");
        }

        var (accessToken, refreshToken) = await _authenticationService.AuthenticateCustomerAsync(authDto.Email, authDto.Password);            
      
        if (string.IsNullOrEmpty(accessToken))
        {
            return Unauthorized("Invalid credentials");
        }

        //return Ok(new { Token = token });
        return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken });
    }
    
    [HttpPost("driverLogin")]
    public async Task<IActionResult> driverLogin([FromBody] AuthDTO authDto)
    {
        if (authDto == null || string.IsNullOrEmpty(authDto.Email) || string.IsNullOrEmpty(authDto.Password))
        {
            return BadRequest("Invalid login request.");
        }

        var (accessToken, refreshToken) = await _authenticationService.AuthenticateDriverAsync(authDto.Email, authDto.Password);            
      
        if (string.IsNullOrEmpty(accessToken))
        {
            return Unauthorized("Invalid credentials");
        }

        //return Ok(new { Token = token });
        return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken });
    }
    
    [HttpPost("refresh-token")]
    public IActionResult RefreshToken([FromBody] RefreshTokenDTO request)
    {
        var principal = _authenticationService.ValidateToken(request.AccessToken);

        if (principal == null)
        {
            return Unauthorized("Invalid or expired access token.");
        }

        var email = principal.FindFirstValue(ClaimTypes.Name);
        var role = principal.FindFirstValue(ClaimTypes.Role);
        var (accessToken, refreshToken)=_authenticationService.NewRefreshToken(email, role,request);

        return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken });
    }

    

}