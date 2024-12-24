using KiloTaxi.API.Helper.Authentication.Interface;
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
    public async Task<IActionResult> Login([FromBody] AdminDTO adminDto)
    {
        if (adminDto == null || string.IsNullOrEmpty(adminDto.Email) || string.IsNullOrEmpty(adminDto.Password))
        {
            return BadRequest("Invalid login request.");
        }

        var accessToken = await _authenticationService.AuthenticateAdminAsync(adminDto.Email, adminDto.Password);            
      
        if (string.IsNullOrEmpty(accessToken))
        {
            return Unauthorized("Invalid credentials");
        }

        //return Ok(new { Token = token });
        return Ok(accessToken);
    }

}