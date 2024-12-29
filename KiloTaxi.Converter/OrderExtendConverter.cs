using System;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;

namespace KiloTaxi.Converter
{
    public static class OrderExtendConverter
    {
        public static OrderExtendDTO ConvertEntityToModel(OrderExtend orderExtendEntity)
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

            return new OrderExtendDTO()
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
            OrderExtendDTO orderExtendDTO,
            ref OrderExtend orderExtendEntity
        )
        {
            try
            {
                if (orderExtendDTO == null)
                {
                    LoggerHelper.Instance.LogError(
                        new ArgumentNullException(nameof(orderExtendDTO)),
                        "OrderExtendDTO is null"
                    );
                    throw new ArgumentNullException(
                        nameof(orderExtendDTO),
                        "Source orderExtendDTO cannot be null"
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

                orderExtendEntity.Id = orderExtendDTO.Id;
                orderExtendEntity.DestinationLocation = orderExtendDTO.DestinationLocation;
                orderExtendEntity.DestinationLat = orderExtendDTO.DestinationLat;
                orderExtendEntity.DestinationLong = orderExtendDTO.DestinationLong;
                orderExtendEntity.CreateDate = orderExtendDTO.CreateDate;
                orderExtendEntity.OrderId = orderExtendDTO.OrderId;
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
