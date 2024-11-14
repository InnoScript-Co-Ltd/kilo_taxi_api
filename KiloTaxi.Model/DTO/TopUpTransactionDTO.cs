using System.ComponentModel.DataAnnotations;

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
    public string TopUpTransactionStatus { get; set; }
    
    public int WalletId { get; set; }
    
    public int PaymentChannelId { get; set; }
}