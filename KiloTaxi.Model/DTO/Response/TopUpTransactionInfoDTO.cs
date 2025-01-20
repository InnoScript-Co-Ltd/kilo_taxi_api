using System.ComponentModel.DataAnnotations;
using KiloTaxi.Common.Enums;
using Microsoft.AspNetCore.Http;

namespace KiloTaxi.Model.DTO.Response;

public class TopUpTransactionInfoDTO
{
    public int Id { get; set; }
    
    public decimal Amount { get; set; }
    
    public string? TransactionScreenShoot { get; set; }
    
    public TopUpTransactionStatus Status { get; set; }
    
    public string? PhoneNumber { get; set; }    
    
    public string? DigitalPaymentFromPhoneNumber { get; set; }
    
    public string? DigitalPaymentToPhoneNumber { get; set; }
    
    public int PaymentChannelId { get; set; }
    
    public IFormFile? File_TransactionScreenShoot { get; set; }
    
    public int UseId { get; set; }
}