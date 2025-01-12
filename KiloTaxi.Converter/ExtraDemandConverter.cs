using System;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;

namespace KiloTaxi.Converter
{
    public static class ExtraDemandConverter
    {
        public static ExtraDemandDTO ConvertEntityToModel(ExtraDemand extraDemandEntity)
        {
            if (extraDemandEntity == null)
            {
                LoggerHelper.Instance.LogError(
                    new ArgumentNullException(nameof(extraDemandEntity)),
                    "ExtraDemand entity is null"
                );
                throw new ArgumentNullException(
                    nameof(extraDemandEntity),
                    "Source extraDemandEntity cannot be null"
                );
            }

            return new ExtraDemandDTO()
            {
                Id = extraDemandEntity.Id,
                Title = extraDemandEntity.Title,
                Description = extraDemandEntity.Description,
                Amount = extraDemandEntity.Amount,
                CreateDate = extraDemandEntity.CreateDate,
            };
        }

        public static void ConvertModelToEntity(
            ExtraDemandDTO extraDemandDTO,
            ref ExtraDemand extraDemandEntity
        )
        {
            try
            {
                if (extraDemandDTO == null)
                {
                    LoggerHelper.Instance.LogError(
                        new ArgumentNullException(nameof(extraDemandDTO)),
                        "ExtraDemandDTO is null"
                    );
                    throw new ArgumentNullException(
                        nameof(extraDemandDTO),
                        "Source extraDemandDTO cannot be null"
                    );
                }

                if (extraDemandEntity == null)
                {
                    LoggerHelper.Instance.LogError(
                        new ArgumentNullException(nameof(extraDemandEntity)),
                        "ExtraDemand entity is null"
                    );
                    throw new ArgumentNullException(
                        nameof(extraDemandEntity),
                        "Target extraDemandEntity cannot be null"
                    );
                }

                extraDemandEntity.Id = extraDemandDTO.Id;
                extraDemandEntity.Title = extraDemandDTO.Title;
                extraDemandEntity.Description = extraDemandDTO.Description;
                extraDemandEntity.Amount = extraDemandDTO.Amount;
                extraDemandEntity.CreateDate = extraDemandDTO.CreateDate;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    "Error during ExtraDemandDTO to ExtraDemand entity conversion"
                );
                throw;
            }
        }
    }
}
