using System;
using KiloTaxi.Common.Enums;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;

namespace KiloTaxi.Converter
{
    public static class PaymentChannelConverter
    {
        public static PaymentChannelDTO ConvertEntityToModel(
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

            return new PaymentChannelDTO
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
            PaymentChannelDTO paymentChannelDTO,
            ref PaymentChannel paymentChannelEntity
        )
        {
            if (paymentChannelDTO == null)
                throw new ArgumentNullException(nameof(paymentChannelDTO));

            if (paymentChannelEntity == null)
                throw new ArgumentNullException(nameof(paymentChannelEntity));

            paymentChannelEntity.Id = paymentChannelDTO.Id;
            paymentChannelEntity.ChannelName = paymentChannelDTO.ChannelName;
            paymentChannelEntity.Description = paymentChannelDTO.Description;
            paymentChannelEntity.PaymentType = paymentChannelDTO.PaymentType.ToString();
            paymentChannelEntity.Icon = paymentChannelDTO.Icon;
            paymentChannelEntity.Phone = paymentChannelDTO.Phone;
            paymentChannelEntity.UserName = paymentChannelDTO.UserName;
        }
    }
}
