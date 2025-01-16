using System;
using KiloTaxi.Common.Enums;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;

namespace KiloTaxi.Converter
{
    public static class PaymentChannelConverter
    {
        public static PaymentChannelInfoDTO ConvertEntityToModel(
            PaymentChannel paymentChannelEntity,
            string mediaHostUrl
        )
        {
            if (paymentChannelEntity == null)
            {
                LoggerHelper.Instance.LogError(
                    new ArgumentNullException(nameof(paymentChannelEntity)),
                    "Payment Channel entity is null"
                );
                throw new ArgumentNullException(
                    nameof(paymentChannelEntity),
                    "Source PaymentChannel entity cannot be null"
                );
            }

            return new PaymentChannelInfoDTO
            {
                Id = paymentChannelEntity.Id,
                ChannelName = paymentChannelEntity.ChannelName,
                Description = paymentChannelEntity.Description,
                PaymentType = Enum.Parse<PaymentType>(paymentChannelEntity.PaymentType),
                Icon = mediaHostUrl + paymentChannelEntity.Icon,
                Phone = paymentChannelEntity.Phone,
                UserName = paymentChannelEntity.UserName,
            };
        }

        public static void ConvertModelToEntity(
            PaymentChannelFormDTO paymentChannelFormDto,
            ref PaymentChannel paymentChannelEntity
        )
        {
            try
            {
                if (paymentChannelFormDto == null)
                {
                    LoggerHelper.Instance.LogError(
                        new ArgumentNullException(nameof(paymentChannelFormDto)),
                        "PaymentChannelDTO model is null"
                    );
                    throw new ArgumentNullException(
                        nameof(paymentChannelFormDto),
                        "Source PaymentChannelDTO model cannot be null"
                    );
                }

                paymentChannelEntity.Id = paymentChannelFormDto.Id;
                paymentChannelEntity.ChannelName = paymentChannelFormDto.ChannelName;
                paymentChannelEntity.Description = paymentChannelFormDto.Description;
                paymentChannelEntity.PaymentType = paymentChannelFormDto.PaymentType.ToString();
                paymentChannelEntity.Icon = paymentChannelFormDto.Icon;
                paymentChannelEntity.Phone = paymentChannelFormDto.Phone;
                paymentChannelEntity.UserName = paymentChannelFormDto.UserName;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    "Unexpected error during model-to-entity conversion"
                );
                throw;
            }
        }
    }
}
