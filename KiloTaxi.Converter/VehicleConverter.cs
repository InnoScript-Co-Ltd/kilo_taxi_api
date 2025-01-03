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

    public static void ConvertModelToEntity(DriverFormDTO driverFormDto, ref Vehicle vehicleEntity)
    {
        try
        {
            if (driverFormDto == null)
            {
                LoggerHelper.Instance.LogError(new ArgumentNullException(nameof(driverFormDto)), "VehicleDTO model is null");
                throw new ArgumentNullException(nameof(driverFormDto), "Source VehicleDTO model cannot be null");
            }
            vehicleEntity.Id = driverFormDto.VehicleId;
            vehicleEntity.VehicleNo = driverFormDto.VehicleNo;
            vehicleEntity.Model = driverFormDto.Model;
            vehicleEntity.FuelType = driverFormDto.FuelType;
            vehicleEntity.VehicleType = driverFormDto.VehicleType;
            vehicleEntity.DriverMode = driverFormDto.DriverMode.ToString();
            vehicleEntity.BusinessLicenseImage = driverFormDto.BusinessLicenseImage;
            vehicleEntity.VehicleLicenseFront = driverFormDto.VehicleLicenseFront;
            vehicleEntity.VehicleLicenseBack = driverFormDto.VehicleLicenseBack;
            vehicleEntity.Status = driverFormDto.VehicleStatus.ToString();
            vehicleEntity.DriverId = driverFormDto.DriverId;
            vehicleEntity.VehicleTypeId = driverFormDto.VehicleTypeId;
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