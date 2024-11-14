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
    
    [Required]
    public string DriverLicense{get;set;}
    
    [Required]
    public string DriverImageLicenseFront{get;set;}
    
    [Required]
    public string DriverImageLicenseBack{get;set;}
    
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
    
    public string Gender{get;set;}
    
    public string DriverStatus{get;set;}
    
    public string KycStatus{get;set;}
}