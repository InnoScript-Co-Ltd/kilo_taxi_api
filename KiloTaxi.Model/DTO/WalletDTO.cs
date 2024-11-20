using System.ComponentModel.DataAnnotations;
using KiloTaxi.Common.Enums;

namespace KiloTaxi.Model.DTO;

public class WalletDTO
{
    public int Id { get; set; }
    
    [Required]
    public string UserName{get;set;}
    
    [Required]
    public string PhoneNo{get;set;}
    
    [Required]
    [EmailAddress]
    public string Email{get;set;}
    
    [Required]
    [Range(0.01, 10000.00)]
    public decimal Balance{get;set;}
    
    [Required]
    public string WalletType{get;set;}
    
    [Required]
    public string WalletStatus{get;set;}
    
    public int DriverId{get;set;}
    
    public int CustomerId{get;set;}
}