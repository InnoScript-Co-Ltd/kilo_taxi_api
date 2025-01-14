using System.ComponentModel.DataAnnotations;
using KiloTaxi.Common.Enums;
using Microsoft.AspNetCore.Http;

namespace KiloTaxi.Model.DTO.Request;

public class DriverCreateFormDTO
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Username is required.")]
    [StringLength(
        30,
        MinimumLength = 6,
        ErrorMessage = "Username must be between 6 and 30 characters."
    )]
    public string Name { get; set; }

    [Required(ErrorMessage = "Phone number is required.")]
    [StringLength(
        10,
        MinimumLength = 8,
        ErrorMessage = "Phone number must be between 8 and 10 digits."
    )]
    [RegularExpression(
        @"^9\d{7,9}$",
        ErrorMessage = "Phone number must start with 9 and be between 8 to 10 digits."
    )]
    public string Phone { get; set; }

    public string? MobilePrefix { get; set; }

    public string? Email { get; set; }

    public DateTime? Dob { get; set; }

    public string? RefreshToken { get; set; }

    public string? Otp { get; set; }

    // public string? NrcImageFront{get; set;}
    //
    // public string? NrcImageBack{get; set;}

    public DateTime? EmailVerifiedAt { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? PhoneVerifiedAt { get; set; }

    [StringLength(
        26,
        MinimumLength = 6,
        ErrorMessage = "Phone number must be between 6 and 26 Characters."
    )]
    public string Password { get; set; }

    public string? State { get; set; }

    public GenderType? Gender { get; set; }

    public DateTime? RefreshTokenExpiryTime { get; set; }

    public string? Profile { get; set; }

    [Required(ErrorMessage = "DriverLicense is required.")]
    [StringLength(
        100,
        MinimumLength = 5,
        ErrorMessage = "DriverLicense number must be between 5 and 100 Characters."
    )]
    public string DriverLicense { get; set; }

    public string? DriverImageLicenseFront { get; set; }

    public string? DriverImageLicenseBack { get; set; }

    // public IFormFile? File_NrcImageFront { get; set; }
    // public IFormFile? File_NrcImageBack { get; set; }
    [Required(ErrorMessage = "DriverImageLicenseFront is required.")]
    public IFormFile? File_DriverImageLicenseFront { get; set; }

    [Required(ErrorMessage = "DriverImageLicenseBack is required.")]
    public IFormFile? File_DriverImageLicenseBack { get; set; }

    [Required(ErrorMessage = "Profile is required.")]
    public IFormFile? File_Profile { get; set; }

    [Required(ErrorMessage = "Role is required.")]
    public string Role { get; set; }

    [Required(ErrorMessage = "Nrc is required.")]
    [StringLength(
        60,
        MinimumLength = 5,
        ErrorMessage = "Nrc number must be between 5 and 60 Characters."
    )]
    public string Nrc { get; set; }

    [Required(ErrorMessage = "City is required.")]
    public string City { get; set; }

    [Required(ErrorMessage = "TownShip is required.")]
    public string TownShip { get; set; }

    [Required(ErrorMessage = "Address is required.")]
    public string Address { get; set; }

    [Required(ErrorMessage = "PropertyStatus is required.")]
    public PropertyStatus PropertyStatus { get; set; }

    [Required(ErrorMessage = "ReferralMobileNumber is required.")]
    [StringLength(
        10,
        MinimumLength = 8,
        ErrorMessage = "ReferralMobileNumber must be between 8 and 10 digits."
    )]
    [RegularExpression(
        @"^9\d{7,9}$",
        ErrorMessage = "ReferralMobileNumber must start with 9 and be between 8 to 10 digits."
    )]
    public string ReferralMobileNumber { get; set; }

    [Required(ErrorMessage = "DriverStatus is required.")]
    public DriverStatus Status { get; set; }

    [Required(ErrorMessage = "AvailableStatus is required.")]
    public DriverStatus AvailableStatus { get; set; }

    [Required(ErrorMessage = "KycStatus is required.")]
    public KycStatus KycStatus { get; set; }

    public int VehicleId { get; set; }

    [Required(ErrorMessage = "VehicleNo is required.")]
    [StringLength(
        30,
        MinimumLength = 3,
        ErrorMessage = "VehicleNo must be between 3 and 30 Characters."
    )]
    public string VehicleNo { get; set; }

    [Required(ErrorMessage = "Model is required.")]
    [StringLength(
        30,
        MinimumLength = 3,
        ErrorMessage = "Model must be between 3 and 30 Characters."
    )]
    public string Model { get; set; }

    [Required(ErrorMessage = "VehicleType is required.")]
    [StringLength(
        30,
        MinimumLength = 3,
        ErrorMessage = "VehicleType must be between 3 and 30 Characters."
    )]
    public string VehicleType { get; set; }

    public string? BusinessLicenseImage { get; set; }

    public string? VehicleLicenseFront { get; set; }

    public string? VehicleLicenseBack { get; set; }

    [Required(ErrorMessage = "VehicleStatus is required.")]
    public VehicleStatus VehicleStatus { get; set; }

    [Required(ErrorMessage = "DriverMode is required.")]
    public DriverMode DriverMode { get; set; }

    public int DriverId { get; set; }

    public int? VehicleTypeId { get; set; }

    [Required(ErrorMessage = "BusinessLicenseImage is required.")]
    public IFormFile? File_BusinessLicenseImage { get; set; }

    [Required(ErrorMessage = "VehicleLicenseFront is required.")]
    public IFormFile? File_VehicleLicenseFront { get; set; }

    [Required(ErrorMessage = "VehicleLicenseBack is required.")]
    public IFormFile? File_VehicleLicenseBack { get; set; }

    [Required(ErrorMessage = "FuelType is required.")]
    public string FuelType { get; set; }
}
