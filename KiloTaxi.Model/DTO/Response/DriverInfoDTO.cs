using System.ComponentModel.DataAnnotations;
using KiloTaxi.Common.Enums;
using Microsoft.AspNetCore.Http;

namespace KiloTaxi.Model.DTO.Response;

public class DriverInfoDTO
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Phone { get; set; }

    public string? Profile { get; set; }

    public string Password { get; set; }

    public DateTime CreatedDate { get; set; }

    public string MobilePrefix { get; set; }

    public string Email { get; set; }

    public string Role { get; set; }
    public DateTime? Dob { get; set; }

    public string NrcImageFront { get; set; }

    public string NrcImageBack { get; set; }

    public string State { get; set; }

    public GenderType Gender { get; set; }

    public string DriverLicense { get; set; }

    public string? DriverImageLicenseFront { get; set; }

    public string? DriverImageLicenseBack { get; set; }

    public string Nrc { get; set; }

    public string City { get; set; }

    public string TownShip { get; set; }

    public string Address { get; set; }

    public PropertyStatus PropertyStatus { get; set; }

    public string ReferralMobileNumber { get; set; }

    public DriverStatus Status { get; set; }

    public DriverStatus AvailableStatus { get; set; }

    public KycStatus KycStatus { get; set; }

    public IEnumerable<VehicleInfoDTO> VehicleInfo { get; set; }
}
