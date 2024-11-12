using System.ComponentModel.DataAnnotations;
using KiloTaxi.Common.Enums;

namespace KiloTaxi.Model.DTO;

public class VehicleDTO
{
    public int Id { get; set; }
    
    [Required]
    public string VehicleNo { get; set; }
    
    [Required]
    public string VehicleType { get; set; }
    
    [Required]
    public string Model { get; set; }
    
    [Required]
    public string FuelType { get; set; }
    
    public string? BusinessLicenseImage { get; set; }
    
    public string? VehicleLicenseFront{get;set;}
    
    public string? VehicleLicenseBack{get;set;}
    
    public VehicleStatus VehicleStatus { get; set; }
    
    public int DriverId { get; set; }
    
}