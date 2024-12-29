using System.Reflection.Metadata.Ecma335;

namespace KiloTaxi.Model.DTO.Response;

public class OtpInfo
{
    public string Otp { get; set; }
    
    public DateTime OtpExpired { get; set; }
    
    public int RetryCount { get; set; }
    
    public DateTime TerminateDate { get; set; }
    
    public string Phone { get; set; }
    public string Email { get; set; }

    public string Password { get; set; }
    
    public string Role { get; set; }
    
    public string UserName { get; set; }
    public string UserStatus{get;set;}
}