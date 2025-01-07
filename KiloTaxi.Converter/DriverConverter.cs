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
            Password=driverEntity.Password,
            PropertyStatus =Enum.Parse<PropertyStatus>(driverEntity.PropertyStatus),
            ReferralMobileNumber= driverEntity.ReferralMobileNumber,
            DriverLicense = driverEntity.DriverLicense,
            DriverImageLicenseFront = mediaHostUrl + driverEntity.DriverImageLicenseFront,
            DriverImageLicenseBack = mediaHostUrl + driverEntity.DriverImageLicenseBack,
            Address = driverEntity.Address,
            State = driverEntity.State,
            City = driverEntity.City,
            TownShip = driverEntity.TownShip,
            AvailableStatus=string.IsNullOrEmpty(driverEntity.AvabilityStatus ) ? DriverStatus.Online : Enum.Parse<DriverStatus>(driverEntity.AvabilityStatus),
            Gender = string.IsNullOrEmpty(driverEntity.Gender ) ? GenderType.Undefined : Enum.Parse<GenderType>(driverEntity.Gender),
            Status = string.IsNullOrEmpty(driverEntity.Status ) ? DriverStatus.Pending : Enum.Parse<DriverStatus>(driverEntity.Status),
            KycStatus = string.IsNullOrEmpty(driverEntity.KycStatus ) ? KycStatus.Pending : Enum.Parse<KycStatus>(driverEntity.KycStatus)
        };

    }

    public static void  ConvertModelToEntity(DriverCreateFormDTO driverCreateFormDto, ref Driver driverEntity)
    {
        try
        {
            if (driverCreateFormDto == null)
            {
                LoggerHelper.Instance.LogError(new ArgumentNullException(nameof(driverCreateFormDto)), "DriverDTO model is null");
                throw new ArgumentNullException(nameof(driverCreateFormDto), "Source DriverDTO model cannot be null");
            }

            driverEntity.Id = driverCreateFormDto.Id;
            driverEntity.Name = driverCreateFormDto.Name;
            driverEntity.Profile = driverCreateFormDto.Profile;
            driverEntity.MobilePrefix = driverCreateFormDto.MobilePrefix;
            driverEntity.Phone = driverCreateFormDto.Phone;
            driverEntity.Email = driverCreateFormDto.Email;
            driverEntity.Dob = driverCreateFormDto.Dob;
            driverEntity.Nrc = driverCreateFormDto.Nrc;
            driverEntity.RefreshToken = driverCreateFormDto.RefreshToken;
            driverEntity.RefreshTokenExpiryTime = driverCreateFormDto.RefreshTokenExpiryTime;
            driverEntity.Otp = driverCreateFormDto.Otp;
            driverEntity.Role = driverCreateFormDto.Role;
            
            driverEntity.DriverLicense = driverCreateFormDto.DriverLicense;
            driverEntity.DriverImageLicenseFront = driverCreateFormDto.DriverImageLicenseFront;
            driverEntity.DriverImageLicenseBack = driverCreateFormDto.DriverImageLicenseBack;
            driverEntity.EmailVerifiedAt = driverCreateFormDto.EmailVerifiedAt;
            driverEntity.PhoneVerifiedAt = driverCreateFormDto.PhoneVerifiedAt;
            driverEntity.Password = driverCreateFormDto.Password;
            driverEntity.Address = driverCreateFormDto.Address;
            driverEntity.State = driverCreateFormDto.State;
            driverEntity.City = driverCreateFormDto.City;
            driverEntity.TownShip = driverCreateFormDto.TownShip;
            driverEntity.Gender = driverCreateFormDto.Gender.ToString();
            driverEntity.PropertyStatus=driverCreateFormDto.PropertyStatus.ToString();
            driverEntity.ReferralMobileNumber = driverCreateFormDto.ReferralMobileNumber;
            driverEntity.AvabilityStatus=driverCreateFormDto.AvailableStatus.ToString();
            driverEntity.Status = driverCreateFormDto.Status.ToString();
            driverEntity.KycStatus = driverCreateFormDto.KycStatus.ToString();

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
    
     public static void  UpdateConvertModelToEntity(DriverUpdateFormDTO driverUpdateFormDto, ref Driver driverEntity)
    {
        try
        {
            if (driverUpdateFormDto == null)
            {
                LoggerHelper.Instance.LogError(new ArgumentNullException(nameof(driverUpdateFormDto)), "DriverDTO model is null");
                throw new ArgumentNullException(nameof(driverUpdateFormDto), "Source DriverDTO model cannot be null");
            }

            driverEntity.Id = driverUpdateFormDto.Id;
            driverEntity.Name = driverUpdateFormDto.Name;
            driverEntity.Profile = driverUpdateFormDto.Profile;
            driverEntity.Phone = driverUpdateFormDto.Phone;
            driverEntity.Dob = driverUpdateFormDto.Dob;
            driverEntity.Nrc = driverUpdateFormDto.Nrc;
            driverEntity.DriverLicense = driverUpdateFormDto.DriverLicense;
            driverEntity.DriverImageLicenseFront = driverUpdateFormDto.DriverImageLicenseFront;
            driverEntity.DriverImageLicenseBack = driverUpdateFormDto.DriverImageLicenseBack;
            driverEntity.Password = driverUpdateFormDto.Password;
            driverEntity.Address = driverUpdateFormDto.Address;
            driverEntity.State = driverUpdateFormDto.State;
            driverEntity.City = driverUpdateFormDto.City;
            driverEntity.TownShip = driverUpdateFormDto.TownShip;
            driverEntity.Gender = driverUpdateFormDto.Gender.ToString();
            driverEntity.PropertyStatus=driverUpdateFormDto.PropertyStatus.ToString();
            driverEntity.ReferralMobileNumber = driverUpdateFormDto.ReferralMobileNumber;
            driverEntity.AvabilityStatus=driverUpdateFormDto.AvailableStatus.ToString();
            driverEntity.Status = driverUpdateFormDto.Status.ToString();
            driverEntity.KycStatus = driverUpdateFormDto.KycStatus.ToString();

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

