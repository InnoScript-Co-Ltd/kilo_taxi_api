using System;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;

namespace KiloTaxi.Converter
{
    public static class OrderExtendConverter
    {
        public static OrderExtendInfoDTO ConvertEntityToModel(OrderExtend orderExtendEntity)
        {
            if (orderExtendEntity == null)
            {
                LoggerHelper.Instance.LogError(
                    new ArgumentNullException(nameof(orderExtendEntity)),
                    "OrderExtend entity is null"
                );
                throw new ArgumentNullException(
                    nameof(orderExtendEntity),
                    "Source orderExtendEntity cannot be null"
                );
            }

            return new OrderExtendInfoDTO()
            {
                Id = orderExtendEntity.Id,
                DestinationLocation = orderExtendEntity.DestinationLocation,
                DestinationLat = orderExtendEntity.DestinationLat,
                DestinationLong = orderExtendEntity.DestinationLong,
                CreateDate = orderExtendEntity.CreateDate,
                OrderId = orderExtendEntity.OrderId,
            };
        }

        public static void ConvertModelToEntity(
            OrderExtendFormDTO orderExtendFormDTO,
            ref OrderExtend orderExtendEntity
        )
        {
            try
            {
                if (orderExtendFormDTO == null)
                {
                    LoggerHelper.Instance.LogError(
                        new ArgumentNullException(nameof(orderExtendFormDTO)),
                        "OrderExtendFormDTO is null"
                    );
                    throw new ArgumentNullException(
                        nameof(orderExtendFormDTO),
                        "Source orderExtendFormDTO cannot be null"
                    );
                }

                if (orderExtendEntity == null)
                {
                    LoggerHelper.Instance.LogError(
                        new ArgumentNullException(nameof(orderExtendEntity)),
                        "OrderExtend entity is null"
                    );
                    throw new ArgumentNullException(
                        nameof(orderExtendEntity),
                        "Target orderExtendEntity cannot be null"
                    );
                }

                orderExtendEntity.Id = orderExtendFormDTO.Id;
                orderExtendEntity.DestinationLocation = orderExtendFormDTO.DestinationLocation;
                orderExtendEntity.DestinationLat = orderExtendFormDTO.DestinationLat;
                orderExtendEntity.DestinationLong = orderExtendFormDTO.DestinationLong;
                orderExtendEntity.CreateDate = orderExtendFormDTO.CreateDate;
                orderExtendEntity.OrderId = orderExtendFormDTO.OrderId;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    "Error during OrderExtendDTO to OrderExtend entity conversion"
                );
                throw;
            }
        }
    }
}
