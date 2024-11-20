using System.ComponentModel.DataAnnotations;
using KiloTaxi.Common.Enums;
using Microsoft.AspNetCore.Http;

namespace KiloTaxi.Model.DTO;

public class DriverDTO
{
    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; }
    
    public string? Profile { get; set; }
    
    [Required]
    public string MobilePrefix{get;set;}
    
    [Required]
    public string Phone{get;set;}
    
    [Required]
    [EmailAddress]
    public string Email{get;set;}
    
    [DataType(DataType.DateTime)]
    public DateTime Dob{get;set;}
    
    [StringLength(50)]
    public string? Nrc{get;set;}
    
    [StringLength(100)]
    public string? NrcImageFront{get;set;}
    
    [StringLength(100)]
    public string? NrcImageBack{get;set;}
    
    public string? DriverLicense{get;set;}
    
    public string? DriverImageLicenseFront{get;set;}
    
    public string? DriverImageLicenseBack{get;set;}
    
    [DataType(DataType.DateTime)]
    public DateTime EmailVerifiedAt{get;set;}
    
    [DataType(DataType.DateTime)]
    public DateTime PhoneVerifiedAt{get;set;}
    
    [Required]
    [MinLength(3)]
    [MaxLength(25)]
    public string Password{get;set;}
    
    [Required]
    public string Address{get;set;}
    
    [Required]
    public string State{get;set;}
    
    [Required]
    public string City{get;set;}
    
    [Required]
    public string TownShip{get;set;}
    
    [Required]
    public string Gender{get;set;}
    
    public string DriverStatus{get;set;}
    
    public string KycStatus{get;set;}
    
    public IEnumerable<VehicleDTO> Vehicle { get; set; }
    public IEnumerable<WalletDTO> Wallet { get; set; }
    
    public IFormFile? File_NrcImageFront { get; set; }
    public IFormFile? File_NrcImageBack { get; set; }
    public IFormFile? File_DriverImageLicenseFront{ get; set; }
    public IFormFile? File_DriverImageLicenseBack{ get; set; }
    public IFormFile? File_Profile{ get; set; }
}