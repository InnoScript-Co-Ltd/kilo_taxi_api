using System.ComponentModel.DataAnnotations;

namespace KiloTaxi.Model.DTO;

public class PromotionUsageDTO
{
    public int Id { get; set; }
    
    public string DiscountApplied { get; set; }
    
    [Required]
    public int WalletTransactionId { get; set; }
    
    [Required]
    public int PromotionId { get; set; }
    
    [Required]
    public int CustomerId { get; set; }
}