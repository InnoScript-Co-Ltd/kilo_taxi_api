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

        var (accessToken, refreshToken) = await _authenticationService.AuthenticateAdminAsync(authDto.EmailOrPhone, authDto.Password);            
      
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
        if (authDto == null || string.IsNullOrEmpty(authDto.EmailOrPhone))
        {
            return BadRequest("Invalid login request.");
        }

        var (accessToken, refreshToken) = await _authenticationService.AuthenticateCustomerAsync(authDto.EmailOrPhone,authDto.Password);            
      
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
        if (authDto == null || string.IsNullOrEmpty(authDto.EmailOrPhone) || string.IsNullOrEmpty(authDto.Password))
        {
            return BadRequest("Invalid login request.");
        }

        var (accessToken, refreshToken) = await _authenticationService.AuthenticateDriverAsync(authDto.EmailOrPhone, authDto.Password);            
      
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
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        ResponseDTO<OtpInfo> response= _customerRepository.FindCustomerAndGenerateOtp(customerFormDto);
       
        var unVerifiedUser = JsonConvert.SerializeObject(response);

        HttpContext.Session.SetString("UnVerifiedUser"+customerFormDto.Phone, unVerifiedUser);
        
       return response;
    }
    
    
    [HttpPost("Otp-verify")]
    public ActionResult<ResponseDTO<CustomerInfoDTO>> VerifyOTP([FromBody] OtpFormDTO otpFormDto)
    {
        var session_UnVerifiedUser = HttpContext.Session.GetString("UnVerifiedUser"+otpFormDto.Phone);
        var unVerifiedUser = JsonConvert.DeserializeObject<ResponseDTO<OtpInfo>>(session_UnVerifiedUser);
        var Password = unVerifiedUser.Payload.Password;
        var Role= unVerifiedUser.Payload.Role;
        var otpCode = unVerifiedUser.Payload.Otp;
        DateTime otpExpired = unVerifiedUser.Payload.OtpExpired; 
            DateTime sessionTerminateDate = unVerifiedUser.Payload.TerminateDate;
            bool isCircuitBreaker = false;
            if (sessionTerminateDate != DateTime.MinValue)
            {
                isCircuitBreaker = true;
            }
        int retryCount = unVerifiedUser.Payload.RetryCount; 
        string phone=unVerifiedUser.Payload.Phone;  
        if (isCircuitBreaker && sessionTerminateDate>DateTime.UtcNow  && otpFormDto.Phone==phone)
        {
            return new ResponseDTO<CustomerInfoDTO>()
            {
                StatusCode = BadRequest().StatusCode,
                Message="Cant't Verify OTP.Try again later" +" At "+  sessionTerminateDate,
            };
        }
        if (retryCount > 5)
        {
            unVerifiedUser.Payload.TerminateDate = DateTime.UtcNow.AddMinutes(1);; // Update terminate date
            HttpContext.Session.SetString("UnVerifiedUser"+otpFormDto.Phone, JsonConvert.SerializeObject(unVerifiedUser)); // Save updated session

            return new ResponseDTO<CustomerInfoDTO>()
            {
                StatusCode = BadRequest().StatusCode,
                Message="Cant't Verify OTP.Try again later",
            };
        }
         if(otpFormDto.Phone != phone || otpFormDto.Otp != otpCode || retryCount>5)
        {
            retryCount += 1;
            unVerifiedUser.Payload.RetryCount = retryCount; // Update retry count in payload
            HttpContext.Session.SetString("UnVerifiedUser"+otpFormDto.Phone, JsonConvert.SerializeObject(unVerifiedUser)); // Save updated session

            return new ResponseDTO<CustomerInfoDTO>()
            {
                StatusCode = BadRequest().StatusCode,
                Message="Invalid otp code"
            };
        }
        if (otpFormDto.Otp ==otpCode && DateTime.UtcNow > otpExpired)
        {
            retryCount += 1;
            unVerifiedUser.Payload.RetryCount = retryCount; // Update retry count in payload
            HttpContext.Session.SetString("UnVerifiedUser"+otpFormDto.Phone, JsonConvert.SerializeObject(unVerifiedUser)); // Save updated session

            return new ResponseDTO<CustomerInfoDTO>()
            {
                StatusCode = BadRequest().StatusCode,
                Message="Your Otp code is expired."
            };
        } 
       
        
        var customerInfoDto= _customerRepository.AddCustomer(new CustomerFormDTO()
        {
            Phone = otpFormDto.Phone,
            Name =  unVerifiedUser.Payload.UserName,
            Password=Password,
            Role=Role
        });
        HttpContext.Session.Remove("UnVerifiedUser"+otpFormDto.Phone);
        return new ResponseDTO<CustomerInfoDTO>()
        {
            StatusCode = Ok().StatusCode,
            Payload = customerInfoDto,
            Message="Customer Register Success"
        };
        
    }
    

}