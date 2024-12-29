using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiloTaxi.EntityFramework.EntityModel;

public class Vehicle
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public string VehicleNo { get; set; }

    [Required]
    public string Model { get; set; }

    [Required]
    public string FuelType { get; set; }

    public string DriverMode { get; set; }

    public string? BusinessLicenseImage { get; set; }

    public string? VehicleLicenseFront { get; set; }

    public string? VehicleLicenseBack { get; set; }
    
    public string VehicleType { get; set; }

    [Required]
    public string Status { get; set; }

    [ForeignKey("Driver")]
    public int DriverId { get; set; }
    public virtual Driver Driver { get; set; }

    [ForeignKey("VehicleType")]
    public int? VehicleTypeId { get; set; }
}
