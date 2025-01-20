using System;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;

namespace KiloTaxi.Converter
{
    public static class VehicleTypeConverter
    {
        public static VehicleTypeInfoDTO ConvertEntityToModel(VehicleType vehicleTypeEntity)
        {
            if (vehicleTypeEntity == null)
            {
                LoggerHelper.Instance.LogError(
                    new ArgumentNullException(nameof(vehicleTypeEntity)),
                    "VehicleType entity is null"
                );
                throw new ArgumentNullException(
                    nameof(vehicleTypeEntity),
                    "Source vehicleTypeEntity cannot be null"
                );
            }

            return new VehicleTypeInfoDTO()
            {
                Id = vehicleTypeEntity.Id,
                Name = vehicleTypeEntity.Name,
                Description = vehicleTypeEntity.Description,
            };
        }

        public static void ConvertModelToEntity(
            VehicleTypeFormDTO vehicleTypeFormDTO,
            ref VehicleType vehicleTypeEntity
        )
        {
            try
            {
                if (vehicleTypeFormDTO == null)
                {
                    LoggerHelper.Instance.LogError(
                        new ArgumentNullException(nameof(vehicleTypeFormDTO)),
                        "vehicleTypeFormDTO is null"
                    );
                    throw new ArgumentNullException(
                        nameof(vehicleTypeFormDTO),
                        "Source vehicleTypeFormDTO cannot be null"
                    );
                }

                if (vehicleTypeEntity == null)
                {
                    LoggerHelper.Instance.LogError(
                        new ArgumentNullException(nameof(vehicleTypeEntity)),
                        "VehicleType entity is null"
                    );
                    throw new ArgumentNullException(
                        nameof(vehicleTypeEntity),
                        "Target vehicleTypeEntity cannot be null"
                    );
                }

                vehicleTypeEntity.Id = vehicleTypeFormDTO.Id;
                vehicleTypeEntity.Name = vehicleTypeFormDTO.Name;
                vehicleTypeEntity.Description = vehicleTypeFormDTO.Description;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    "Error during VehicleTypeDTO to Review entity conversion"
                );
                throw;
            }
        }
    }
}
