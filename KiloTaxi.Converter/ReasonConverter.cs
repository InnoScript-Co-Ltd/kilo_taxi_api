using System;
using KiloTaxi.Common.Enums;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;

namespace KiloTaxi.Converter
{
    public static class ReasonConverter
    {
        public static ReasonDTO ConvertEntityToModel(Reason reasonEntity)
        {
            if (reasonEntity == null)
            {
                LoggerHelper.Instance.LogError(
                    new ArgumentNullException(nameof(reasonEntity)),
                    "Reason entity is null"
                );
                throw new ArgumentNullException(
                    nameof(reasonEntity),
                    "Source reasonEntity cannot be null"
                );
            }

            return new ReasonDTO()
            {
                Id = reasonEntity.Id,
                Name = reasonEntity.Name,
                Status = Enum.Parse<GeneralStatus>(reasonEntity.Status),
                
            };
        }

        public static void ConvertModelToEntity(ReasonDTO reasonDTO, ref Reason reasonEntity)
        {
            try
            {
                if (reasonDTO == null)
                {
                    LoggerHelper.Instance.LogError(
                        new ArgumentNullException(nameof(reasonDTO)),
                        "reasonDTO is null"
                    );
                    throw new ArgumentNullException(
                        nameof(reasonDTO),
                        "Source reasonDTO cannot be null"
                    );
                }

                if (reasonEntity == null)
                {
                    LoggerHelper.Instance.LogError(
                        new ArgumentNullException(nameof(reasonEntity)),
                        "Reason entity is null"
                    );
                    throw new ArgumentNullException(
                        nameof(reasonEntity),
                        "Target reasonEntity cannot be null"
                    );
                }

                reasonEntity.Id = reasonDTO.Id;
                reasonEntity.Name = reasonDTO.Name;
                reasonEntity.Status = reasonDTO.Status.ToString();
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    "Error during reasonDTO to Reason entity conversion"
                );
                throw;
            }
        }
    }
}
