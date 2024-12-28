using KiloTaxi.Common.Enums;
using Microsoft.AspNetCore.Http;

namespace KiloTaxi.Model.DTO.Request;

public class CustomerFormDTO
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public string? Profile { get; set; }

    public string? MobilePrefix { get; set; }
    
    public string Phone { get; set; }

    public string Role {get;set;}

    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpiryTime { get; set; }
    
    public string? Otp {get;set;}
    
    public string? Address { get; set; }

    public string? City { get; set; }

    public string? Township { get; set; }
    
    public string? Email { get; set; }
    
    public DateTime? EmailVerifiedAt { get; set; }
    
    public DateTime? PhoneVerifiedAt { get; set; }
    
    public string? Password { get; set; }
    
    public string? State { get; set; }

    public GenderType? Gender { get; set; }
    
    public CustomerStatus Status { get; set; }
    
    public KycStatus KycStatus { get; set; }

    public IFormFile? File_Profile { get; set; }
    
}