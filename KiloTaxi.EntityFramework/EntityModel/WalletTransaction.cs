using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiloTaxi.EntityFramework.EntityModel;

public class WalletTransaction
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Column(TypeName = "decimal(18, 2)")]
    public decimal? Amount { get; set; }
    
    [Column(TypeName = "decimal(18, 2)")]
    public decimal BalanceBefore { get; set; }
    
    [Column(TypeName = "decimal(18, 2)")]
    public decimal BalanceAfter { get; set; }
    
    [DataType(DataType.DateTime)]
    public DateTime TransactionDate { get; set; }
    
    public int ReferenceId { get; set; }
    
    [StringLength(500)]
    public string? Details { get; set; }
    
    [Required]
    public string TransactionType { get; set; }
    
    [ForeignKey("WalletUserMapping")]
    public int WalletUserMappingId { get; set; }
    public virtual WalletUserMapping WalletUserMapping { get; set; }
}