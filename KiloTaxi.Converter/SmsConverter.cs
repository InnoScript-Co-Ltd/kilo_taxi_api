using System;
using KiloTaxi.Common.Enums;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;

namespace KiloTaxi.Converter
{
    public static class SmsConverter
    {
        public static SmsDTO ConvertEntityToModel(Sms smsEntity)
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

            return new SmsDTO()
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

        public static void ConvertModelToEntity(SmsDTO smsDTO, ref Sms smsEntity)
        {
            try
            {
                if (smsDTO == null)
                {
                    LoggerHelper.Instance.LogError(
                        new ArgumentNullException(nameof(smsDTO)),
                        "smsDTO is null"
                    );
                    throw new ArgumentNullException(nameof(smsDTO), "Source smsDTO cannot be null");
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

                smsEntity.Id = smsDTO.Id;
                smsEntity.Name = smsDTO.Name;
                smsEntity.MobileNumber = smsDTO.MobileNumber;
                smsEntity.Title = smsDTO.Title;
                smsEntity.Message = smsDTO.Message;
                smsEntity.Status = smsDTO.Status.ToString();
                smsEntity.AdminId = smsDTO.AdminId;
                smsEntity.CustomerId = smsDTO.CustomerId;
                smsEntity.DriverId = smsDTO.DriverId;
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
