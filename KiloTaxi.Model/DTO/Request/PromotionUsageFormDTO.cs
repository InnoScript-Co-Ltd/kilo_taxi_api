using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiloTaxi.Model.DTO.Request;

public class PromotionUsageFormDTO
{
    public int Id { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal DiscountApplied { get; set; }

    [Required]
    public int WalletTransactionId { get; set; }

    [Required]
    public int PromotionId { get; set; }

    [Required]
    public int CustomerId { get; set; }
}
