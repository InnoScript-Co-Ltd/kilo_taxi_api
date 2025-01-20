using System;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;

namespace KiloTaxi.Converter
{
    public static class ExtraDemandConverter
    {
        public static ExtraDemandInfoDTO ConvertEntityToModel(ExtraDemand extraDemandEntity)
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

            return new ExtraDemandInfoDTO()
            {
                Id = extraDemandEntity.Id,
                Title = extraDemandEntity.Title,
                Description = extraDemandEntity.Description,
                Amount = extraDemandEntity.Amount,
                CreateDate = extraDemandEntity.CreateDate,
            };
        }

        public static void ConvertModelToEntity(
            ExtraDemandFormDTO extraDemandFormDTO,
            ref ExtraDemand extraDemandEntity
        )
        {
            try
            {
                if (extraDemandFormDTO == null)
                {
                    LoggerHelper.Instance.LogError(
                        new ArgumentNullException(nameof(extraDemandFormDTO)),
                        "extraDemandFormDTO is null"
                    );
                    throw new ArgumentNullException(
                        nameof(extraDemandFormDTO),
                        "Source extraDemandFormDTO cannot be null"
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

                extraDemandEntity.Id = extraDemandFormDTO.Id;
                extraDemandEntity.Title = extraDemandFormDTO.Title;
                extraDemandEntity.Description = extraDemandFormDTO.Description;
                extraDemandEntity.Amount = extraDemandFormDTO.Amount;
                extraDemandEntity.CreateDate = extraDemandFormDTO.CreateDate;
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
