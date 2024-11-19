using KiloTaxi.Common.Enums;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;

namespace KiloTaxi.Converter;

public static class DriverConverter
{
    public static DriverDTO ConvertEntityToModel(Driver driverEntity, string mediaHostUrl)
    {
        if (driverEntity == null)
        {
            LoggerHelper.Instance.LogError(
                new ArgumentNullException(nameof(driverEntity)),
                "Driver entity is null"
            );
            throw new ArgumentNullException(
                nameof(driverEntity),
                "Source Driver entity cannot be null"
            );
        }

        return new DriverDTO()
        {
            Id = driverEntity.Id,
            Name = driverEntity.Name,
            Profile = mediaHostUrl + driverEntity.Profile,
            MobilePrefix = driverEntity.MobilePrefix,
            Phone = driverEntity.Phone,
            Email = driverEntity.Email,
            Dob = driverEntity.Dob,
            Nrc = driverEntity.Nrc,
            NrcImageFront = mediaHostUrl + driverEntity.NrcImageFront,
            NrcImageBack = mediaHostUrl + driverEntity.NrcImageBack,
            DriverLicense = driverEntity.DriverLicense,
            DriverImageLicenseFront = mediaHostUrl + driverEntity.DriverImageLicenseFront,
            DriverImageLicenseBack = mediaHostUrl + driverEntity.DriverImageLicenseBack,
            EmailVerifiedAt = driverEntity.EmailVerifiedAt,
            PhoneVerifiedAt = driverEntity.PhoneVerifiedAt,
            Address = driverEntity.Address,
            State = driverEntity.State,
            City = driverEntity.City,
            TownShip = driverEntity.TownShip,
            Gender = Enum.Parse<GenderType>(driverEntity.Gender),
            DriverStatus = Enum.Parse<DriverStatus>(driverEntity.DriverStatus),
            KycStatus = Enum.Parse<KycStatus>(driverEntity.KycStatus),
        };
    }

    public static void ConvertModelToEntity(DriverDTO driverDTO, ref Driver driverEntity)
    {
        try
        {
            if (driverDTO == null)
            {
                LoggerHelper.Instance.LogError(
                    new ArgumentNullException(nameof(driverDTO)),
                    "DriverDTO model is null"
                );
                throw new ArgumentNullException(
                    nameof(driverDTO),
                    "Source DriverDTO model cannot be null"
                );
            }

            driverEntity.Id = driverDTO.Id;
            driverEntity.Name = driverDTO.Name;
            driverEntity.Profile = driverDTO.Profile;
            driverEntity.MobilePrefix = driverDTO.MobilePrefix;
            driverEntity.Phone = driverDTO.Phone;
            driverEntity.Email = driverDTO.Email;
            driverEntity.Dob = driverDTO.Dob;
            driverEntity.Nrc = driverDTO.Nrc;
            driverEntity.NrcImageFront = driverDTO.NrcImageFront;
            driverEntity.NrcImageBack = driverDTO.NrcImageBack;
            driverEntity.DriverLicense = driverDTO.DriverLicense;
            driverEntity.DriverImageLicenseFront = driverDTO.DriverImageLicenseFront;
            driverEntity.DriverImageLicenseBack = driverDTO.DriverImageLicenseBack;
            driverEntity.EmailVerifiedAt = driverDTO.EmailVerifiedAt;
            driverEntity.PhoneVerifiedAt = driverDTO.PhoneVerifiedAt;
            driverEntity.Password = driverDTO.Password;
            driverEntity.Address = driverDTO.Address;
            driverEntity.State = driverDTO.State;
            driverEntity.City = driverDTO.City;
            driverEntity.TownShip = driverDTO.TownShip;
            driverEntity.Gender = driverDTO.Gender.ToString();
            driverEntity.DriverStatus = driverDTO.DriverStatus.ToString();
            driverEntity.KycStatus = driverDTO.KycStatus.ToString();
        }
        catch (ArgumentException ex)
        {
            LoggerHelper.Instance.LogError(
                ex,
                "Argument exception during model-to-entity conversion"
            );
            throw;
        }
        catch (Exception ex)
        {
            LoggerHelper.Instance.LogError(
                ex,
                "Unexpected error during model-to-entity conversion"
            );
            throw;
        }
    }
}
