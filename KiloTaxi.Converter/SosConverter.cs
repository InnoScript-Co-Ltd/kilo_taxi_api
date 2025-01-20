using KiloTaxi.Common.Enums;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;

namespace KiloTaxi.Converter;

public static class SosConverter
{
     public static SosInfoDTO ConvertEntityToModel(Sos sosEntity)
        {
            if (sosEntity == null)
            {
                LoggerHelper.Instance.LogError(new ArgumentNullException(nameof(sosEntity)), "Sos entity is null");
                throw new ArgumentNullException(nameof(sosEntity), "Source Sos entity cannot be null");
            }

            return new SosInfoDTO()
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
        
        public static void ConvertModelToEntity(SosFormDTO sosFormDTO, ref Sos sosEntity)
        {
            try
            {
                if (sosFormDTO == null)
                {
                    LoggerHelper.Instance.LogError(new ArgumentNullException(nameof(sosFormDTO)), "sosFormDTO model is null");
                    throw new ArgumentNullException(nameof(sosFormDTO), "Source sosFormDTO model cannot be null");
                }

                sosEntity.Id = sosFormDTO.Id;
                sosEntity.Address = sosFormDTO.Address;
                sosEntity.Status = sosFormDTO.Status.ToString();
                sosEntity.ReferenceId = sosFormDTO.ReferenceId;
                sosEntity.UserType = sosFormDTO.UserType.ToString();
                sosEntity.ReasonId = sosFormDTO.ReasonId;
                
                

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