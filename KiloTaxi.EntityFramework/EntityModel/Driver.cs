using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;

namespace KiloTaxi.EntityFramework.EntityModel;

public class Driver
{
    [Key]
    [DatabaseGenerated((DatabaseGeneratedOption.Identity))]
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public string? Profile { get; set; }
    
    public string MobilePrefix{get;set;}
    
    public string Phone{get;set;}
    
    [Required]
    [EmailAddress]
    public string Email{get;set;}
    
    public DateTime? Dob{get;set;}
    
    public string Nrc{get;set;}
    
    public string? NrcImageFront{get;set;}
    
    public string? NrcImageBack{get;set;}
    public string? DriverLicense{get;set;}
    
    public string? DriverImageLicenseFront{get;set;}
    
    public string? DriverImageLicenseBack{get;set;}
    
    public DateTime? EmailVerifiedAt{get;set;}
    
    public DateTime? PhoneVerifiedAt{get;set;}
    
    [MinLength(3)]
    [MaxLength(25)]
    public string Password{get;set;}
    
    public string Address{get;set;}
    
    public string State{get;set;}
    
    public string City{get;set;}
    
    public string AvabilityStatus {get;set;}
    
    public string TownShip{get;set;}
    
    public string Gender{get;set;}
    
    public string Status{get;set;}
    
    public string Role{get;set;}
    public string KycStatus{get;set;}
}