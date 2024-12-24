using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiloTaxi.EntityFramework.EntityModel;

public class OrderExtend
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    public string DestinationLocation {get;set;}
    
    public string DestinationLat { get; set; }
    
    public string DestinationLong { get; set; }
    
    [DataType(DataType.DateTime)]
    public DateTime CreateDate { get; set; }
    
    [ForeignKey("Order")]
    public int OrderId { get; set; }
    public virtual Order Order { get; set; }
}