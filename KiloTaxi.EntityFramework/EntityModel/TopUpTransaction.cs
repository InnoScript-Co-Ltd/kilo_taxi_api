using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiloTaxi.EntityFramework.EntityModel;

public class TopUpTransaction
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Amount { get; set; }
    
    [Required]
    public string TransactionScreenShoot { get; set; }
    
    [Required]
    public string Status { get; set; }
    
    [ForeignKey("Wallet")]
    public int WalletId { get; set; }
    public virtual Wallet Wallet { get; set; }
    
    // [ForeignKey("PaymentChannel")]
    // public int PaymentChannelId { get; set; }
    // public virtual PaymentChannel PaymentChannel { get; set; }    
}