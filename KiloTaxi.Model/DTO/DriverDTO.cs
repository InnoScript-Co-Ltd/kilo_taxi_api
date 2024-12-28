using System.ComponentModel.DataAnnotations;
using KiloTaxi.Common.Enums;
using Microsoft.AspNetCore.Http;

namespace KiloTaxi.Model.DTO;

public class DriverDTO
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public string? Profile { get; set; }
    
    public string MobilePrefix{get;set;}
    
    public string Phone{get;set;}
    
    public string Role {get;set;}
    
    [Required]
    [EmailAddress]
    public string Email{get;set;}
    
    [DataType(DataType.DateTime)]
    public DateTime? Dob{get;set;}
    
    public string Nrc{get;set;}
    
    [StringLength(100)]
    public string? NrcImageFront{get;set;}
    
    [StringLength(100)]
    public string? NrcImageBack{get;set;}
    
    public string? DriverLicense{get;set;}
    
    public string? DriverImageLicenseFront{get;set;}
    
    public string? DriverImageLicenseBack{get;set;}
    
    [DataType(DataType.DateTime)]
    public DateTime? EmailVerifiedAt{get;set;}
    
    [DataType(DataType.DateTime)]
    public DateTime? PhoneVerifiedAt{get;set;}
    
    [MinLength(3)]
    [MaxLength(25)]
    public string Password{get;set;}
    
    public string? RefreshToken { get; set; }
        
    [DataType(DataType.Date)]
    public DateTime? RefreshTokenExpiryTime { get; set; }
    
    public string? Otp {get;set;}

    
    public string Address{get;set;}
    
    public string State{get;set;}
    
    public string City{get;set;}
    
    public string TownShip{get;set;}
    
    public GenderType Gender{get;set;}
    
    public DriverStatus Status{get;set;}
    
    public DriverStatus AvailableStatus{get;set;}
    public KycStatus KycStatus{get;set;}
    
    public IEnumerable<VehicleDTO>? Vehicle { get; set; }
    public IEnumerable<WalletUserMappingDTO>? WalletUserMapping { get; set; }
    
    public IFormFile? File_NrcImageFront { get; set; }
    public IFormFile? File_NrcImageBack { get; set; }
    public IFormFile? File_DriverImageLicenseFront{ get; set; }
    public IFormFile? File_DriverImageLicenseBack{ get; set; }
    public IFormFile? File_Profile{ get; set; }
}