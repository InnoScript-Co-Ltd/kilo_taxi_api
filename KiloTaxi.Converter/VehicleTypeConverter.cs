using System;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;

namespace KiloTaxi.Converter
{
    public static class VehicleTypeConverter
    {
        public static VehicleTypeDTO ConvertEntityToModel(VehicleType vehicleTypeEntity)
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

            return new VehicleTypeDTO()
            {
                Id = vehicleTypeEntity.Id,
                Name = vehicleTypeEntity.Name,
                Description = vehicleTypeEntity.Description,
            };
        }

        public static void ConvertModelToEntity(
            VehicleTypeDTO vehicleTypeDTO,
            ref VehicleType vehicleTypeEntity
        )
        {
            try
            {
                if (vehicleTypeDTO == null)
                {
                    LoggerHelper.Instance.LogError(
                        new ArgumentNullException(nameof(vehicleTypeDTO)),
                        "vehicleTypeDTO is null"
                    );
                    throw new ArgumentNullException(
                        nameof(vehicleTypeDTO),
                        "Source vehicleTypeDTO cannot be null"
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

                vehicleTypeEntity.Id = vehicleTypeDTO.Id;
                vehicleTypeEntity.Name = vehicleTypeDTO.Name;
                vehicleTypeEntity.Description = vehicleTypeDTO.Description;
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
