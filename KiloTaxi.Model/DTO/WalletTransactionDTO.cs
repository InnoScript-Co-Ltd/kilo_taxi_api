using KiloTaxi.Common.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiloTaxi.Model.DTO;

public class WalletTransactionDTO
{
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
    public string Details { get; set; }
    
    [Required]
    public TransactionType TransactionType { get; set; }
    
    public int WalletUserMappingId { get; set; }
}