using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiloTaxi.EntityFramework.EntityModel;

public class PaymentChannel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    public string AccountOwnerName { get; set; }
    
    [Required]
    public string PhoneNo { get; set; }
    
    [Required]
    public string BankNo { get; set; }
    
    [Required]
    public string BankLogo { get; set; }
    
    [Required]
    public string PaymentMethod { get; set; }
    
    [Required]
    public string AccountNo { get; set; }
    
    [Required]
    public string GeneralStatus { get; set; }
    
    
}