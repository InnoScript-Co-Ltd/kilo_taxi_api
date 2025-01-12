using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiloTaxi.EntityFramework.EntityModel;

public class OrderExtraDemand
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [ForeignKey("Order")]
    public int OrderId { get; set; }
    public virtual Order Order { get; set; }
    
    public int Unit { get; set; }

    
    [ForeignKey("ExtraDemand")]
    public int ExtraDemandId { get; set; }
    public virtual ExtraDemand ExtraDemand { get; set; }
    
}