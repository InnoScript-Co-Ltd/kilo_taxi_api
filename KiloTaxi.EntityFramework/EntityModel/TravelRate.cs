using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiloTaxi.EntityFramework.EntityModel;

public class TravelRate
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    public string Unit { get; set; }
    
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Rate { get; set; }
    
    [ForeignKey("VehicleType")]
    public int VehicleTypeId { get; set; }
    public virtual VehicleType VehicleType { get; set; }
    
    [ForeignKey("City")]
    public int CityId { get; set; }
    public virtual City City { get; set; }
}