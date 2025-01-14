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
                    "Payment Channel Entity is null"
                );
                throw new ArgumentNullException(
                    nameof(paymentChannelEntity),
                    "Source paymentChannelEntity cannot be null"
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
            PaymentChannelFormDTO paymentChannelFormDTO,
            ref PaymentChannel paymentChannelEntity
        )
        {
            if (paymentChannelFormDTO == null)
                throw new ArgumentNullException(nameof(paymentChannelFormDTO));

            if (paymentChannelEntity == null)
                throw new ArgumentNullException(nameof(paymentChannelEntity));

            paymentChannelEntity.Id = paymentChannelFormDTO.Id;
            paymentChannelEntity.ChannelName = paymentChannelFormDTO.ChannelName;
            paymentChannelEntity.Description = paymentChannelFormDTO.Description;
            paymentChannelEntity.PaymentType = paymentChannelFormDTO.PaymentType.ToString();
            paymentChannelEntity.Icon = paymentChannelFormDTO.Icon;
            paymentChannelEntity.Phone = paymentChannelFormDTO.Phone;
            paymentChannelEntity.UserName = paymentChannelFormDTO.UserName;
        }
    }
}
