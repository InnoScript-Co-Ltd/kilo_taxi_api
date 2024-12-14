using System;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Model.DTO;

namespace KiloTaxi.Converter
{
    public static class PaymentChannelConverter
    {
        public static PaymentChannelDTO ConvertEntityToModel(PaymentChannel paymentChannel)
        {
            if (paymentChannel == null)
                throw new ArgumentNullException(nameof(paymentChannel));

            return new PaymentChannelDTO
            {
                Id = paymentChannel.Id,
                ChannelName = paymentChannel.ChannelName,
                Description = paymentChannel.Description,
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
        }
    }
}
