using System.ComponentModel.DataAnnotations;
using KiloTaxi.Common.Enums;

namespace KiloTaxi.Model.DTO;

public class PaymentChannelDTO
{
    public int Id { get; set; }
    
    [Required]
    public string AccountOwnerName { get; set; }
    
    [Required]
    public string PhoneNo { get; set; }
    
    [Required]
    public string BankNo { get; set; }
    
    [Required]
    public string BankLogo { get; set; }
    
    [Required]
    public PaymentMethod PaymentMethod { get; set; }
    
    [Required]
    public string AccountNo { get; set; }
    
    [Required]
    public string GeneralStatus { get; set; }
}