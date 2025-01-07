using System.ComponentModel.DataAnnotations;
using KiloTaxi.Common.Enums;
using Microsoft.AspNetCore.Http;

namespace KiloTaxi.Model.DTO.Request;

public class DriverUpdateFormDTO
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Username is required.")]
    [StringLength(30, MinimumLength = 6, ErrorMessage = "Username must be between 6 and 30 characters.")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "Phone number is required.")]
    [StringLength(10, MinimumLength = 8, ErrorMessage = "Phone number must be between 8 and 10 digits.")]
    [RegularExpression(@"^9\d{7,9}$", ErrorMessage = "Phone number must start with 9 and be between 8 to 10 digits.")]
    public string Phone { get; set; }
    
    public DateTime? Dob { get; set; }

    [StringLength(60, MinimumLength = 6, ErrorMessage = "Password number must be between 6 and 26 Characters.")]
    public string Password {get; set;}
    
    public string? Profile { get; set; }

    
    public string? State {get; set;}
    
    public GenderType? Gender{get;set;}
    
    [Required(ErrorMessage = "DriverLicense is required.")]
    [StringLength(100, MinimumLength = 5, ErrorMessage = "DriverLicense number must be between 5 and 100 Characters.")]
    public string DriverLicense{get;set;}
    
    public string? DriverImageLicenseFront{get;set;}
    
    public string? DriverImageLicenseBack{get;set;}
    
    public IFormFile? File_DriverImageLicenseFront { get; set; }
    
    public IFormFile? File_DriverImageLicenseBack{ get; set; }
    
    public IFormFile? File_Profile{ get; set; }
    
    [Required(ErrorMessage = "Nrc is required.")]
    [StringLength(60, MinimumLength = 5, ErrorMessage = "Nrc number must be between 5 and 60 Characters.")]
    public string Nrc{get;set;}
    
    [Required(ErrorMessage = "City is required.")]
    public string City{get;set;}
    
    [Required(ErrorMessage = "PropertyStatus is required.")]
    public PropertyStatus PropertyStatus{get;set;}

    [Required(ErrorMessage = "TownShip is required.")]
    public string TownShip{get;set;}

    [Required(ErrorMessage = "Address is required.")]
    public string Address{get;set;}
    
    [Required(ErrorMessage = "ReferralMobileNumber is required.")]
    [StringLength(10, MinimumLength = 8, ErrorMessage = "ReferralMobileNumber must be between 8 and 10 digits.")]
    [RegularExpression(@"^9\d{7,9}$", ErrorMessage = "ReferralMobileNumber must start with 9 and be between 8 to 10 digits.")]
    public string ReferralMobileNumber{get;set;}
    
    [Required(ErrorMessage = "DriverStatus is required.")]
    public DriverStatus Status{get;set;}
    
    [Required(ErrorMessage = "AvailableStatus is required.")]
    public DriverStatus AvailableStatus{get;set;}
    
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }

    
    [Required(ErrorMessage = "KycStatus is required.")]
    public KycStatus KycStatus{get;set;}

}