using System.ComponentModel.DataAnnotations;
using KiloTaxi.Common.Enums;
using Microsoft.AspNetCore.Http;

namespace KiloTaxi.Model.DTO.Request;

public class CustomerFormDTO
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Username is required.")]
    [StringLength(30, MinimumLength = 6, ErrorMessage = "Username must be between 6 and 30 characters.")]
    public string Name { get; set; }
    
    public string? Profile { get; set; }

    public string? MobilePrefix { get; set; }
    
    [Required(ErrorMessage = "Phone number is required.")]
    [StringLength(10, MinimumLength = 8, ErrorMessage = "Phone number must be between 8 and 10 digits.")]
    [RegularExpression(@"^9\d{7,9}$", ErrorMessage = "Phone number must start with 9 and be between 8 to 10 digits.")]
    public string Phone { get; set; }

    public string? Role {get;set;}

    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpiryTime { get; set; }
    
    public string? Otp {get;set;}
    
    public string? Address { get; set; }

    public string? City { get; set; }

    public string? Township { get; set; }
    
    public string? Email { get; set; }
    
    public DateTime? EmailVerifiedAt { get; set; }
    
    public DateTime? PhoneVerifiedAt { get; set; }
    
    [StringLength(26, MinimumLength = 6, ErrorMessage = "Phone number must be between 6 and 26 Characters.")]
    public string Password { get; set; }
    
    public string? State { get; set; }

    public GenderType? Gender { get; set; }
    
    public CustomerStatus Status { get; set; }
    
    public KycStatus KycStatus { get; set; }

    public IFormFile? File_Profile { get; set; }
    
}