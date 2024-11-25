using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiloTaxi.EntityFramework.EntityModel;

public class PromotionUsage
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Column(TypeName = "decimal(18, 2)")]
    public decimal DiscountApplied { get; set; }
    
    [ForeignKey("WalletTransaction")]
    public int WalletTransactionId { get; set; }
    public WalletTransaction WalletTransaction { get; set; }
    
    [ForeignKey("Promotion")]
    public int PromotionId { get; set; }
    public Promotion Promotion { get; set; }
    
    [ForeignKey("Customer")]
    public int CustomerId { get; set; }
    public Customer Customer { get; set; }
    
    
}