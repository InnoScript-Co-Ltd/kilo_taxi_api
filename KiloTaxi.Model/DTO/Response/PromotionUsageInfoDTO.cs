using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiloTaxi.Model.DTO.Response;

public class PromotionUsageInfoDTO
{
    public int Id { get; set; }

    public decimal DiscountApplied { get; set; }

    public int WalletTransactionId { get; set; }

    public int PromotionId { get; set; }

    public int CustomerId { get; set; }
}
