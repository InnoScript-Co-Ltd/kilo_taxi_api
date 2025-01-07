using KiloTaxi.Common.Enums;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;

namespace KiloTaxi.Converter;

public static class VehicleConverter
{
    public static VehicleInfoDTO ConvertEntityToModel(Vehicle vehicleEntity, string mediaHostUrl)
    {
        if (vehicleEntity == null)
        {
            LoggerHelper.Instance.LogError(new ArgumentNullException(nameof(vehicleEntity)), "Vehicle entity is null");
            throw new ArgumentNullException(nameof(vehicleEntity), "Source Vehicle entity cannot be null");
        }

        return new VehicleInfoDTO()
        {
            Id = vehicleEntity.Id,
            VehicleNo = vehicleEntity.VehicleNo,
            Model = vehicleEntity.Model,
            FuelType = vehicleEntity.FuelType,
            VehicleType = vehicleEntity.VehicleType,
            DriverMode = Enum.Parse<DriverMode>(vehicleEntity.DriverMode),
            BusinessLicenseImage = mediaHostUrl + vehicleEntity.BusinessLicenseImage,
            VehicleLicenseFront = mediaHostUrl + vehicleEntity.VehicleLicenseFront,
            VehicleLicenseBack = mediaHostUrl + vehicleEntity.VehicleLicenseBack,
            Status = Enum.Parse<VehicleStatus>(vehicleEntity.Status),
            VehicleTypeId = vehicleEntity.VehicleTypeId,
            DriverName = vehicleEntity.Driver.Name
        };
    }

    public static void ConvertModelToEntity(DriverCreateFormDTO driverCreateFormDto, ref Vehicle vehicleEntity)
    {
        try
        {
            if (driverCreateFormDto == null)
            {
                LoggerHelper.Instance.LogError(new ArgumentNullException(nameof(driverCreateFormDto)), "VehicleDTO model is null");
                throw new ArgumentNullException(nameof(driverCreateFormDto), "Source VehicleDTO model cannot be null");
            }
            vehicleEntity.Id = driverCreateFormDto.VehicleId;
            vehicleEntity.VehicleNo = driverCreateFormDto.VehicleNo;
            vehicleEntity.Model = driverCreateFormDto.Model;
            vehicleEntity.FuelType = driverCreateFormDto.FuelType;
            vehicleEntity.VehicleType = driverCreateFormDto.VehicleType;
            vehicleEntity.DriverMode = driverCreateFormDto.DriverMode.ToString();
            vehicleEntity.BusinessLicenseImage = driverCreateFormDto.BusinessLicenseImage;
            vehicleEntity.VehicleLicenseFront = driverCreateFormDto.VehicleLicenseFront;
            vehicleEntity.VehicleLicenseBack = driverCreateFormDto.VehicleLicenseBack;
            vehicleEntity.Status = driverCreateFormDto.VehicleStatus.ToString();
            vehicleEntity.DriverId = driverCreateFormDto.DriverId;
            vehicleEntity.VehicleTypeId = driverCreateFormDto.VehicleTypeId;
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
    public static void UpdateConvertModelToEntity(VehicleUpdateFormDTO vehicleUpdateFormDTO, ref Vehicle vehicleEntity)
    {
        try
        {
            if (vehicleUpdateFormDTO == null)
            {
                LoggerHelper.Instance.LogError(new ArgumentNullException(nameof(vehicleUpdateFormDTO)), "VehicleDTO model is null");
                throw new ArgumentNullException(nameof(vehicleUpdateFormDTO), "Source VehicleDTO model cannot be null");
            }
            vehicleEntity.Id = vehicleUpdateFormDTO.Id;
            vehicleEntity.VehicleNo = vehicleUpdateFormDTO.VehicleNo;
            vehicleEntity.Model = vehicleUpdateFormDTO.Model;
            vehicleEntity.FuelType = vehicleUpdateFormDTO.FuelType;
            vehicleEntity.VehicleType = vehicleUpdateFormDTO.VehicleType;
            vehicleEntity.DriverMode = vehicleUpdateFormDTO.DriverMode.ToString();
            vehicleEntity.BusinessLicenseImage = vehicleUpdateFormDTO.BusinessLicenseImage;
            vehicleEntity.VehicleLicenseFront = vehicleUpdateFormDTO.VehicleLicenseFront;
            vehicleEntity.VehicleLicenseBack = vehicleUpdateFormDTO.VehicleLicenseBack;
            vehicleEntity.Status = vehicleUpdateFormDTO.Status.ToString();
            vehicleEntity.DriverId = vehicleUpdateFormDTO.DriverId;
            vehicleEntity.VehicleTypeId = vehicleUpdateFormDTO.VehicleTypeId;
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