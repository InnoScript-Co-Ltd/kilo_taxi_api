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
    [HttpPost("logout")]
    public async Task<ResponseDTO<string>> Logout([FromHeader(Name = "Authorization")] string authorizationHeader)
    {           
        ResponseDTO<string> response = new ResponseDTO<string>();
        if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
        {
            response.Message = "Invalid authorization header";
            response.StatusCode = 400;
            return response;
        }

        var token = authorizationHeader.Substring("Bearer ".Length).Trim();

        try
        {
            // Call the logout service to blacklist the token
            await _authenticationService.LogoutAsync(token);
            response.Message = "Logout successful.";
            response.StatusCode = 200;
            return response;
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            response.StatusCode = 500;
            return response;
        }
    }

    
    [HttpPost("customerLogin")]
    public async Task<IActionResult> customerLogin([FromBody] AuthDTO authDto)
    {
        if (authDto == null || string.IsNullOrEmpty(authDto.EmailOrPhone))
        {
            return BadRequest("Invalid login request.");
        }

        var (accessToken, refreshToken,customerId) = await _authenticationService.AuthenticateCustomerAsync(authDto.EmailOrPhone,authDto.Password);            
      
        if (string.IsNullOrEmpty(accessToken))
        {
            return Unauthorized("Invalid credentials");
        }

        //return Ok(new { Token = token });
        return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken, customerId = customerId });
    }
    
    [HttpPost("driverLogin")]
    public async Task<IActionResult> driverLogin([FromBody] AuthDTO authDto)
    {
        if (authDto == null || string.IsNullOrEmpty(authDto.EmailOrPhone) || string.IsNullOrEmpty(authDto.Password))
        {
            return BadRequest("Invalid login request.");
        }

        var (accessToken, refreshToken,driverId) = await _authenticationService.AuthenticateDriverAsync(authDto.EmailOrPhone, authDto.Password);            
      
        if (string.IsNullOrEmpty(accessToken))
        {
            return Unauthorized("Invalid credentials");
        }

        //return Ok(new { Token = token });
        return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken,driverId = driverId });
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
        if (isCircuitBreaker && sessionTerminateDate>DateTime.Now  && otpFormDto.Phone==phone)
        {
            unVerifiedUser.Payload.RetryCount = 0; // Update retry count in payload
            HttpContext.Session.SetString("UnVerifiedUser"+otpFormDto.Phone, JsonConvert.SerializeObject(unVerifiedUser)); 
            return new ResponseDTO<CustomerInfoDTO>()
            {
                StatusCode = BadRequest().StatusCode,
                Message="Cant't Verify OTP.Try again later" +" At "+  sessionTerminateDate,
                TimeStamp = DateTime.Now
            };
        }
        if (retryCount >= 5)
        {
            unVerifiedUser.Payload.TerminateDate = DateTime.Now.AddMinutes(1);; // Update terminate date
            HttpContext.Session.SetString("UnVerifiedUser"+otpFormDto.Phone, JsonConvert.SerializeObject(unVerifiedUser)); // Save updated session

            return new ResponseDTO<CustomerInfoDTO>()
            {
                StatusCode = BadRequest().StatusCode,
                Message="Cant't Verify OTP.Try again later",
                TimeStamp = DateTime.Now
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
        if (otpFormDto.Otp ==otpCode && DateTime.Now > otpExpired)
        {
            retryCount += 1;
            unVerifiedUser.Payload.RetryCount = retryCount; // Update retry count in payload
            HttpContext.Session.SetString("UnVerifiedUser"+otpFormDto.Phone, JsonConvert.SerializeObject(unVerifiedUser)); // Save updated session

            return new ResponseDTO<CustomerInfoDTO>()
            {
                StatusCode = BadRequest().StatusCode,
                Message="Your Otp code is expired.",
                TimeStamp = DateTime.Now
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
    
    [HttpPost("resend-otp")]
    public async Task<ActionResult<ResponseDTO<OtpInfo>>> resendOtp([FromQuery] string phone)
    {
        var session_UnVerifiedUser = HttpContext.Session.GetString("UnVerifiedUser"+phone);
        ResponseDTO<OtpInfo> responseDto = new ResponseDTO<OtpInfo>();

        if (session_UnVerifiedUser == null)
        {
            responseDto.StatusCode = BadRequest().StatusCode;
            responseDto.Message = "phone number is invalid";
            return responseDto;
        }
        
        var unVerifiedUser = JsonConvert.DeserializeObject<ResponseDTO<OtpInfo>>(session_UnVerifiedUser);
        var OtpCode =  _authenticationService.GenerateOtp();
        unVerifiedUser.Payload.Otp = OtpCode;    
       HttpContext.Session.SetString("UnVerifiedUser"+phone,JsonConvert.SerializeObject(unVerifiedUser));
       var smsApi = _configuration.GetSection("SmsApi");
       var BaseUrl = smsApi["BaseUrl"];
       var apiKey = smsApi["ApiKey"];
       var apiSecret = smsApi["ApiSecret"];
       string credentials = $"{apiKey}:{apiSecret}";
       _customerRepository.sendSms(phone, OtpCode, credentials, BaseUrl);
       responseDto.StatusCode = Ok().StatusCode;
       responseDto.Message = "Generate OTP Resend Success.";
        return responseDto;
    }
    

}