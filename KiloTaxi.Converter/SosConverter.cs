using KiloTaxi.Common.Enums;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;

namespace KiloTaxi.Converter;

public static class SosConverter
{
     public static SosDTO ConvertEntityToModel(Sos sosEntity)
        {
            if (sosEntity == null)
            {
                LoggerHelper.Instance.LogError(new ArgumentNullException(nameof(sosEntity)), "Sos entity is null");
                throw new ArgumentNullException(nameof(sosEntity), "Source Sos entity cannot be null");
            }

            return new SosDTO
            {
                Id = sosEntity.Id,
                Address = sosEntity.Address,
                Status = Enum.Parse<GeneralStatus>(sosEntity.Status),
                ReferenceId = sosEntity.ReferenceId,
                UserType = Enum.Parse<UserType>(sosEntity.UserType),
                ReasonId = sosEntity.ReasonId,
                ReasonName=sosEntity.Reason.Name,
            };
        }
        
        public static void ConvertModelToEntity(SosDTO sosDTO, ref Sos sosEntity)
        {
            try
            {
                if (sosDTO == null)
                {
                    LoggerHelper.Instance.LogError(new ArgumentNullException(nameof(sosDTO)), "SosDTO model is null");
                    throw new ArgumentNullException(nameof(sosDTO), "Source SosDTO model cannot be null");
                }

                sosEntity.Id = sosDTO.Id;
                sosEntity.Address = sosDTO.Address;
                sosEntity.Status = sosDTO.Status.ToString();
                sosEntity.ReferenceId = sosDTO.ReferenceId;
                sosEntity.UserType = sosDTO.UserType.ToString();
                sosEntity.ReasonId = sosDTO.ReasonId;
                
                

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