using KiloTaxi.Common.Enums;
using Microsoft.AspNetCore.Http;

namespace KiloTaxi.Model.DTO.Request;

public class DriverFormDTO
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public string Phone { get; set; }
    
    public string? MobilePrefix {get; set;}
    
    public string? Email { get; set; }
    
    public DateTime? Dob { get; set; }
    
    public string? RefreshToken { get; set; }
    
    public string? Otp{get; set;}
    
    // public string? NrcImageFront{get; set;}
    //
    // public string? NrcImageBack{get; set;}
    
    public DateTime? EmailVerifiedAt{get; set;}
    
    public DateTime? PhoneVerifiedAt{get; set;}
    
    public string? Password {get; set;}
    
    public string? State {get; set;}
    
    public GenderType? Gender{get;set;}

    
    public DateTime? RefreshTokenExpiryTime { get; set; }
    
    public string? Profile { get; set; }
    
    public string DriverLicense{get;set;}
    
    public string? DriverImageLicenseFront{get;set;}
    
    public string? DriverImageLicenseBack{get;set;}
    
    // public IFormFile? File_NrcImageFront { get; set; }
    // public IFormFile? File_NrcImageBack { get; set; }
    public IFormFile? File_DriverImageLicenseFront { get; set; }
    
    public IFormFile? File_DriverImageLicenseBack{ get; set; }

    public IFormFile? File_Profile{ get; set; }

    public string Role {get;set;}
    
    public string Nrc{get;set;}

    public string City{get;set;}

    public string TownShip{get;set;}

    public string Address{get;set;}
    
    public PropertyStatus PropertyStatus{get;set;}
    
    public string ReferralMobileNumber{get;set;}
    
    public DriverStatus Status{get;set;}
    
    public DriverStatus AvailableStatus{get;set;}
    
    public KycStatus KycStatus{get;set;}
    
    public int VehicleId{get;set;}
    
    public string VehicleNo{get;set;}
    
    public string Model { get; set; }
    
    public string VehicleType{get;set;}
    
    public string? BusinessLicenseImage { get; set; }

    public string? VehicleLicenseFront { get; set; }

    public string? VehicleLicenseBack { get; set; }
    
    public VehicleStatus VehicleStatus{get;set;}
    
    public DriverMode DriverMode { get; set; }
    
    public int DriverId { get; set; }
    
    public int? VehicleTypeId { get; set; }
        
    public IFormFile? File_BusinessLicenseImage { get; set; }
    
    public IFormFile? File_VehicleLicenseFront { get; set; }
    
    public IFormFile? File_VehicleLicenseBack { get; set; }
    
    public string FuelType { get; set; }

}