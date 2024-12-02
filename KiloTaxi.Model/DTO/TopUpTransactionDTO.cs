﻿using System.ComponentModel.DataAnnotations;
using KiloTaxi.Common.Enums;

namespace KiloTaxi.Model.DTO;

public class TopUpTransactionDTO
{
    public int Id { get; set; }
    
    [Required]
    [Range(0.01, 10000.000)]
    public decimal Amount { get; set; }
    
    [Required]
    public string TransactionScreenShoot { get; set; }
    
    [Required]
    public TopUpTransactionStatus Status { get; set; }
    
    public string? PhoneNumber { get; set; }
    
    public string? DigitalPaymentFromPhoneNumber { get; set; }
    
    public string? DigitalPaymentToPhoneNumber { get; set; }
    
    public int PaymentChannelId { get; set; }
}