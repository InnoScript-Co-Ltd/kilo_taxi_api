using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiloTaxi.EntityFramework.EntityModel;

public class Sos
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]    
    public string Address { get; set; }
    
    [Required]
    public string Status  { get; set; }
    
    public int ReferenceId { get; set; }
    [Required]
    public string WalletType { get; set; }
    
    [ForeignKey("Reason")]
    public int ReasonId { get; set; }
    public virtual Reason Reason { get; set; }
    
    
}