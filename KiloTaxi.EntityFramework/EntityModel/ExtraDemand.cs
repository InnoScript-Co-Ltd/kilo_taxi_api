using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiloTaxi.EntityFramework.EntityModel;

public class ExtraDemand
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    public string Title {get;set;}
    
    public string Description { get; set; }
    
    
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Amount { get; set; }
    
    [DataType(DataType.DateTime)]
    public DateTime CreateDate { get; set; }
    
    public ICollection<OrderExtraDemand> OrderExtraDemand { get; set; }
   
}