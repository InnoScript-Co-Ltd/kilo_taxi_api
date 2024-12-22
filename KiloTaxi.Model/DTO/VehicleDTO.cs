using System.ComponentModel.DataAnnotations;
using KiloTaxi.Common.Enums;
using Microsoft.AspNetCore.Http;

namespace KiloTaxi.Model.DTO;

public class VehicleDTO
{
    public int Id { get; set; }

    [Required]
    public string VehicleNo { get; set; }

    [Required]
    public string Model { get; set; }

    [Required]
    public string FuelType { get; set; }

    public DriverMode DriverMode { get; set; }

    public string? BusinessLicenseImage { get; set; }

    public string? VehicleLicenseFront { get; set; }

    public string? VehicleLicenseBack { get; set; }

    [Required]
    public VehicleStatus Status { get; set; }

    [Required]
    public int DriverId { get; set; }

    public int VehicleTypeId { get; set; }
    public string DriverName { get; set; }
    public IFormFile? File_BusinessLicenseImage { get; set; }
    public IFormFile? File_VehicleLicenseFront { get; set; }
    public IFormFile? File_VehicleLicenseBack { get; set; }
}
