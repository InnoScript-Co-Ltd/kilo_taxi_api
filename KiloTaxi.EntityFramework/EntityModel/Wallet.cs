using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiloTaxi.EntityFramework.EntityModel;

public class Wallet
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set;}
    
    [Required]
    public string UserName{get;set;}
    
    [Required]
    public string PhoneNo{get;set;}
    
    [Required]
    public string Email{get;set;}
    
    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Balance{get;set;}
    
    [Required]
    public string WalletType{get;set;}
    
    [Required]
    public string WalletStatus{get;set;}
    
    [ForeignKey("Driver")]
    public int DriverId{get;set;}
    public virtual Driver Driver{get;set;}
    
    [ForeignKey("Customer")]
    public int CustomerId{get;set;}
    public virtual Customer Customer{get;set;}
    
}