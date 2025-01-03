using KiloTaxi.Common.Enums;

namespace KiloTaxi.Model.DTO.Response;

public class VehicleInfoDTO
{
    public int Id { get; set; }
    
    public string VehicleNo{get;set;}
    
    public string Model { get; set; }
    
    public string VehicleType{get;set;}
    
    public string FuelType { get; set; }
    
    public DriverMode DriverMode { get; set; }
    
    public string? BusinessLicenseImage { get; set; }

    public string? VehicleLicenseFront { get; set; }

    public string? VehicleLicenseBack { get; set; }
    
    public int? VehicleTypeId { get; set; }
    
    public VehicleStatus Status { get; set; }
    
    public string DriverName { get; set; }


}