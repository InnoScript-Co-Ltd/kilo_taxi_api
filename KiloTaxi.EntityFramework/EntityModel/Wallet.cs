using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiloTaxi.EntityFramework.EntityModel;

public class Wallet
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set;}
    
    [Required]
    public string WalletName{get;set;}
    public DateTime CreatedDate{get;set;} 
    public DateTime? UpdateDate{get;set;}

}