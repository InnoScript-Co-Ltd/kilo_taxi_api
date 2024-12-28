using KiloTaxi.Common.Enums;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;

namespace KiloTaxi.Converter;

public static class DriverConverter
{
    public static DriverInfoDTO ConvertEntityToModel(Driver driverEntity, string mediaHostUrl)
    {
        if (driverEntity == null)
        {
            LoggerHelper.Instance.LogError(new ArgumentNullException(nameof(driverEntity)), "Driver entity is null");
            throw new ArgumentNullException(nameof(driverEntity), "Source Driver entity cannot be null");
        }

        return new DriverInfoDTO()
        {
            Id = driverEntity.Id,
            Name = driverEntity.Name,
            Profile = mediaHostUrl +driverEntity.Profile,
            MobilePrefix = driverEntity.MobilePrefix,
            Phone = driverEntity.Phone,
            Email = driverEntity.Email,
            Dob = driverEntity.Dob,
            Nrc = driverEntity.Nrc,
            Role = driverEntity.Role,
            NrcImageFront = mediaHostUrl + driverEntity.NrcImageFront,
            PropertyStatus =Enum.Parse<PropertyStatus>(driverEntity.PropertyStatus),
            ReferralMobileNumber= driverEntity.ReferralMobileNumber,
            NrcImageBack = mediaHostUrl + driverEntity.NrcImageBack,
            DriverLicense = driverEntity.DriverLicense,
            DriverImageLicenseFront = mediaHostUrl + driverEntity.DriverImageLicenseFront,
            DriverImageLicenseBack = mediaHostUrl + driverEntity.DriverImageLicenseBack,
            Address = driverEntity.Address,
            State = driverEntity.State,
            City = driverEntity.City,
            TownShip = driverEntity.TownShip,
            AvailableStatus=string.IsNullOrEmpty(driverEntity.Status ) ? DriverStatus.Online : Enum.Parse<DriverStatus>(driverEntity.Status),
            Gender = string.IsNullOrEmpty(driverEntity.Gender ) ? GenderType.Undefined : Enum.Parse<GenderType>(driverEntity.Gender),
            Status = string.IsNullOrEmpty(driverEntity.Status ) ? DriverStatus.Pending : Enum.Parse<DriverStatus>(driverEntity.Status),
            KycStatus = string.IsNullOrEmpty(driverEntity.KycStatus ) ? KycStatus.Pending : Enum.Parse<KycStatus>(driverEntity.KycStatus)
        };

    }

    public static void  ConvertModelToEntity(DriverFormDTO driverFormDto, ref Driver driverEntity)
    {
        try
        {
            if (driverFormDto == null)
            {
                LoggerHelper.Instance.LogError(new ArgumentNullException(nameof(driverFormDto)), "DriverDTO model is null");
                throw new ArgumentNullException(nameof(driverFormDto), "Source DriverDTO model cannot be null");
            }

            driverEntity.Id = driverFormDto.Id;
            driverEntity.Name = driverFormDto.Name;
            driverEntity.Profile = driverFormDto.Profile;
            driverEntity.MobilePrefix = driverFormDto.MobilePrefix;
            driverEntity.Phone = driverFormDto.Phone;
            driverEntity.Email = driverFormDto.Email;
            driverEntity.Dob = driverFormDto.Dob;
            driverEntity.Nrc = driverFormDto.Nrc;
            driverEntity.RefreshToken = driverFormDto.RefreshToken;
            driverEntity.RefreshTokenExpiryTime = driverFormDto.RefreshTokenExpiryTime;
            driverEntity.Otp = driverFormDto.Otp;
            driverEntity.Role = driverFormDto.Role;
            driverEntity.NrcImageFront = driverFormDto.NrcImageFront;
            driverEntity.NrcImageBack = driverFormDto.NrcImageBack;
            driverEntity.DriverLicense = driverFormDto.DriverLicense;
            driverEntity.DriverImageLicenseFront = driverFormDto.DriverImageLicenseFront;
            driverEntity.DriverImageLicenseBack = driverFormDto.DriverImageLicenseBack;
            driverEntity.EmailVerifiedAt = driverFormDto.EmailVerifiedAt;
            driverEntity.PhoneVerifiedAt = driverFormDto.PhoneVerifiedAt;
            driverEntity.Password = driverFormDto.Password;
            driverEntity.Address = driverFormDto.Address;
            driverEntity.State = driverFormDto.State;
            driverEntity.City = driverFormDto.City;
            driverEntity.TownShip = driverFormDto.TownShip;
            driverEntity.Gender = driverFormDto.Gender.ToString();
            driverEntity.PropertyStatus=driverFormDto.PropertyStatus.ToString();
            driverEntity.ReferralMobileNumber = driverFormDto.ReferralMobileNumber;
            driverEntity.AvabilityStatus=driverFormDto.AvailableStatus.ToString();
            driverEntity.Status = driverFormDto.Status.ToString();
            driverEntity.KycStatus = driverFormDto.KycStatus.ToString();

        }
        catch (ArgumentException ex)
        {
            LoggerHelper.Instance.LogError(ex, "Argument exception during model-to-entity conversion");
            throw;
        }
        catch (Exception ex)
        {
            LoggerHelper.Instance.LogError(ex, "Unexpected error during model-to-entity conversion");
            throw;
        }
    }   
}

