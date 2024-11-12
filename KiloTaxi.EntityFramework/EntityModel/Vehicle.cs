using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiloTaxi.EntityFramework.EntityModel;

public class Vehicle
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    public string VehicleNo { get; set; }
    
    public string VehicleType { get; set; }
    
    public string Model { get; set; }
    
    public string FuelType { get; set; }
    
    public string BusinessLicenseImage { get; set; }
    
    public string VehicleLicenseFront { get; set; }
    
    public string VehicleLicenseBack { get; set; }
    
    public string VehicleStatus { get; set; }
    
    [ForeignKey("Driver")]
    public int DriverId { get; set; }
    public virtual Driver Driver { get; set; }
}