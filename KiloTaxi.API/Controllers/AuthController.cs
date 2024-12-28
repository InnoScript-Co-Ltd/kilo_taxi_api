using System.Security.Claims;
using Azure;
using KiloTaxi.API.Helper.Authentication.Interface;
using KiloTaxi.DataAccess.Interface;
using KiloTaxi.EntityFramework;
using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace KiloTaxi.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IConfiguration _configuration;
    private readonly ICustomerRepository _customerRepository;


    public AuthController(IAuthenticationService authenticationService, IConfiguration configuration,ICustomerRepository customerRepository)
    {
        _authenticationService = authenticationService;
        _configuration = configuration;
        _customerRepository = customerRepository;
    }
    
    [HttpPost("adminLogin")]
    public async Task<IActionResult> Login([FromBody] AuthDTO authDto)
    {
        if (authDto == null || string.IsNullOrEmpty(authDto.EmailOrPhone) || string.IsNullOrEmpty(authDto.Password))
        {
            return BadRequest("Invalid login request.");
        }

        var (accessToken, refreshToken,otp) = await _authenticationService.AuthenticateAdminAsync(authDto.EmailOrPhone, authDto.Password);            
      
        if (string.IsNullOrEmpty(accessToken))
        {
            return Unauthorized("Invalid credentials");
        }

        //return Ok(new { Token = token });
        return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken,OtpCode=otp });
    }
    
    [HttpPost("customerLogin")]
    public async Task<IActionResult> customerLogin([FromBody] AuthDTO authDto)
    {
        if (authDto == null || string.IsNullOrEmpty(authDto.EmailOrPhone))
        {
            return BadRequest("Invalid login request.");
        }

        var (accessToken, refreshToken,otp) = await _authenticationService.AuthenticateCustomerAsync(authDto.EmailOrPhone,authDto.Password);            
      
        if (string.IsNullOrEmpty(accessToken))
        {
            return Unauthorized("Invalid credentials");
        }

        //return Ok(new { Token = token });
        return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken,OtpCode=otp });
    }
    
    [HttpPost("driverLogin")]
    public async Task<IActionResult> driverLogin([FromBody] AuthDTO authDto)
    {
        if (authDto == null || string.IsNullOrEmpty(authDto.EmailOrPhone) || string.IsNullOrEmpty(authDto.Password))
        {
            return BadRequest("Invalid login request.");
        }

        var (accessToken, refreshToken,otp) = await _authenticationService.AuthenticateDriverAsync(authDto.EmailOrPhone, authDto.Password);            
      
        if (string.IsNullOrEmpty(accessToken))
        {
            return Unauthorized("Invalid credentials");
        }

        //return Ok(new { Token = token });
        return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken,OtpCode=otp });
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
    // [HttpPost("Otp-verify")]
    // public IActionResult VerifyOTP([FromBody] RefreshTokenDTO request)
    // {
    //     if (string.IsNullOrEmpty(request.AccessToken) || string.IsNullOrEmpty(request.Otp))
    //         return BadRequest("Token and OTP are required.");
    //    
    //     var vaildOtp=_authenticationService.VarifiedOpt(request.AccessToken, request.Otp);
    //     if (vaildOtp)
    //     {
    //         return Ok("OTP verified successfully.");
    //
    //     } return BadRequest("Invalid OTP.");
    //
    // }

    [HttpPost("CustomerRegister")]
    public ActionResult<ResponseDTO<OtpInfo>> CustomerRegister([FromBody] CustomerFormDTO customerFormDto)
    {
       var response= _customerRepository.FindCustomerAndGenerateOtp(customerFormDto);
       
       HttpContext.Session.SetString("otpCode",response.Payload.Otp);
       HttpContext.Session.SetString("otpExpired", response.Payload.OtpExpired.ToString("o")); 
       HttpContext.Session.SetString("Phone", response.Payload.Phone);
       HttpContext.Session.SetInt32("RetryCount", response.Payload.RetryCount);
       return response;
    }
    
    
    [HttpPost("Otp-verify")]
    public ActionResult<ResponseDTO<CustomerInfoDTO>> VerifyOTP([FromBody] OtpFormDTO OtpFormDto)
    {
        var otpCode = HttpContext.Session.GetString("otpCode");
        DateTime otpExpired= DateTime.Parse(HttpContext.Session.GetString("otpExpired"));
        DateTime sessionTerminateDate= DateTime.Parse(HttpContext.Session.GetString("TerminateDate") ?? "");
        int retryCount = HttpContext.Session.GetInt32("RetryCount") ?? 0; // Default to 0 if not set

        string phone=HttpContext.Session.GetString("Phone");    
        if (sessionTerminateDate>DateTime.Now && OtpFormDto.Phone==phone)
        {
            return new ResponseDTO<CustomerInfoDTO>()
            {
                StatusCode = BadRequest().StatusCode,
                Message="Cant't Verify OTP.Try again later",
            };
        }
        else if(OtpFormDto.Phone != phone || OtpFormDto.Otp != otpCode)
        {
            retryCount += 1;
            HttpContext.Session.SetInt32("RetryCount",retryCount);
            return new ResponseDTO<CustomerInfoDTO>()
            {
                StatusCode = BadRequest().StatusCode,
                Message="Invalid otp code"
            };
        }else if (OtpFormDto.Otp ==otpCode && DateTime.UtcNow > otpExpired)
        {
            retryCount += 1;
            HttpContext.Session.SetInt32("RetryCount",retryCount);
            return new ResponseDTO<CustomerInfoDTO>()
            {
                StatusCode = BadRequest().StatusCode,
                Message="Your Otp code is expired."
            };
        } else if (retryCount > 5)
         {
             DateTime terminateDate = DateTime.UtcNow.AddMinutes(1);
             HttpContext.Session.SetString("TerminateDate",terminateDate.ToString("o")); 
             return new ResponseDTO<CustomerInfoDTO>()
             {
                 StatusCode = BadRequest().StatusCode,
                 Message="Cant't Verify OTP.Try again later",
             };
         }
        
        var customerInfoDto= _customerRepository.AddCustomer(new CustomerFormDTO()
        {
            Phone = OtpFormDto.Phone,
            Name =  OtpFormDto.Name
        });
        return new ResponseDTO<CustomerInfoDTO>()
        {
            StatusCode = Ok().StatusCode,
            Payload = customerInfoDto,
            Message="Customer Register Success"
        };
    }
    

}