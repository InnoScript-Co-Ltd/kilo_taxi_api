using System;
using KiloTaxi.Common.Enums;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;

namespace KiloTaxi.Converter
{
    public static class ReasonConverter
    {
        public static ReasonInfoDTO ConvertEntityToModel(Reason reasonEntity)
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

            return new ReasonInfoDTO()
            {
                Id = reasonEntity.Id,
                Name = reasonEntity.Name,
                Status = Enum.Parse<GeneralStatus>(reasonEntity.Status),
                
            };
        }

        public static void ConvertModelToEntity(ReasonFormDTO reasonFormDTO, ref Reason reasonEntity)
        {
            try
            {
                if (reasonFormDTO == null)
                {
                    LoggerHelper.Instance.LogError(
                        new ArgumentNullException(nameof(reasonFormDTO)),
                        "reasonDTO is null"
                    );
                    throw new ArgumentNullException(
                        nameof(reasonFormDTO),
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

                reasonEntity.Id = reasonFormDTO.Id;
                reasonEntity.Name = reasonFormDTO.Name;
                reasonEntity.Status = reasonFormDTO.Status.ToString();
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
