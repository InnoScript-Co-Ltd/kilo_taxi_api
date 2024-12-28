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
            DriverMode = Enum.Parse<DriverMode>(vehicleEntity.DriverMode),
            BusinessLicenseImage = mediaHostUrl + vehicleEntity.BusinessLicenseImage,
            VehicleLicenseFront = mediaHostUrl + vehicleEntity.VehicleLicenseFront,
            VehicleLicenseBack = mediaHostUrl + vehicleEntity.VehicleLicenseBack,
            Status = Enum.Parse<VehicleStatus>(vehicleEntity.Status),
            VehicleTypeId = vehicleEntity.VehicleTypeId,
        };
    }

    public static void ConvertModelToEntity(DriverFormDTO vehicleDTO, ref Vehicle vehicleEntity)
    {
        try
        {
            if (vehicleDTO == null)
            {
                LoggerHelper.Instance.LogError(new ArgumentNullException(nameof(vehicleDTO)), "VehicleDTO model is null");
                throw new ArgumentNullException(nameof(vehicleDTO), "Source VehicleDTO model cannot be null");
            }
            vehicleEntity.Id = vehicleDTO.Id;
            vehicleEntity.VehicleNo = vehicleDTO.VehicleNo;
            vehicleEntity.Model = vehicleDTO.Model;
            vehicleEntity.FuelType = vehicleDTO.FuelType;
            vehicleEntity.DriverMode = vehicleDTO.DriverMode.ToString();
            vehicleEntity.BusinessLicenseImage = vehicleDTO.BusinessLicenseImage;
            vehicleEntity.VehicleLicenseFront = vehicleDTO.VehicleLicenseFront;
            vehicleEntity.VehicleLicenseBack = vehicleDTO.VehicleLicenseBack;
            vehicleEntity.Status = vehicleDTO.Status.ToString();
            vehicleEntity.DriverId = vehicleDTO.DriverId;
            vehicleEntity.VehicleTypeId = vehicleDTO.VehicleTypeId;
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