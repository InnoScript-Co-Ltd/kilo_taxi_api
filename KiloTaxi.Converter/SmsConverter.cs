using System;
using KiloTaxi.Common.Enums;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;

namespace KiloTaxi.Converter
{
    public static class SmsConverter
    {
        public static SmsInfoDTO ConvertEntityToModel(Sms smsEntity)
        {
            if (smsEntity == null)
            {
                LoggerHelper.Instance.LogError(
                    new ArgumentNullException(nameof(smsEntity)),
                    "Sms entity is null"
                );
                throw new ArgumentNullException(
                    nameof(smsEntity),
                    "Source smsEntity cannot be null"
                );
            }

            return new SmsInfoDTO()
            {
                Id = smsEntity.Id,
                Name = smsEntity.Name,
                MobileNumber = smsEntity.MobileNumber,
                Title = smsEntity.Title,
                Message = smsEntity.Message,
                Status = Enum.Parse<SmsStatus>(smsEntity.Status),
                AdminId = smsEntity.AdminId,
                AdminName = smsEntity.Admin.Name,
                CustomerId = smsEntity.CustomerId,
                CustomerName = smsEntity.Customer.Name,
                DriverId = smsEntity.DriverId,
                DriverName = smsEntity.Driver.Name,
            };
        }

        public static void ConvertModelToEntity(SmsFormDTO smsFormDTO, ref Sms smsEntity)
        {
            try
            {
                if (smsFormDTO == null)
                {
                    LoggerHelper.Instance.LogError(
                        new ArgumentNullException(nameof(smsFormDTO)),
                        "smsFormDTO is null"
                    );
                    throw new ArgumentNullException(nameof(smsFormDTO), "Source smsDTO cannot be null");
                }

                if (smsEntity == null)
                {
                    LoggerHelper.Instance.LogError(
                        new ArgumentNullException(nameof(smsEntity)),
                        "Sms entity is null"
                    );
                    throw new ArgumentNullException(
                        nameof(smsEntity),
                        "Target smsEntity cannot be null"
                    );
                }

                smsEntity.Id = smsFormDTO.Id;
                smsEntity.Name = smsFormDTO.Name;
                smsEntity.MobileNumber = smsFormDTO.MobileNumber;
                smsEntity.Title = smsFormDTO.Title;
                smsEntity.Message = smsFormDTO.Message;
                smsEntity.Status = smsFormDTO.Status.ToString();
                smsEntity.AdminId = smsFormDTO.AdminId;
                smsEntity.CustomerId = smsFormDTO.CustomerId;
                smsEntity.DriverId = smsFormDTO.DriverId;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    "Error during SmsDTO to Review entity conversion"
                );
                throw;
            }
        }
    }
}
