using System.ComponentModel.DataAnnotations;
using KiloTaxi.Common.Enums;
using Microsoft.AspNetCore.Http;

namespace KiloTaxi.Model.DTO;

public class TopUpTransactionDTO
{
    public int Id { get; set; }
    
    [Required]
    [Range(0.01, 10000.000)]
    public decimal Amount { get; set; }
    
    public string? TransactionScreenShoot { get; set; }
    
    [Required]
    public TopUpTransactionStatus Status { get; set; }
    
    public string? PhoneNumber { get; set; }
    
    public string? DigitalPaymentFromPhoneNumber { get; set; }
    
    public string? DigitalPaymentToPhoneNumber { get; set; }
    
    public int PaymentChannelId { get; set; }
    
    public IFormFile? File_TransactionScreenShoot { get; set; }
    public int UseId { get; set; }
}