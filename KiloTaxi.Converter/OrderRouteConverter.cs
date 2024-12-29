using System;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;

namespace KiloTaxi.Converter
{
    public static class OrderRouteConverter
    {
        public static OrderRouteDTO ConvertEntityToModel(OrderRoute orderRouteEntity)
        {
            if (orderRouteEntity == null)
            {
                LoggerHelper.Instance.LogError(
                    new ArgumentNullException(nameof(orderRouteEntity)),
                    "OrderRoute entity is null"
                );
                throw new ArgumentNullException(
                    nameof(orderRouteEntity),
                    "Source orderRouteEntity cannot be null"
                );
            }

            return new OrderRouteDTO()
            {
                Id = orderRouteEntity.Id,
                Lat = orderRouteEntity.Lat,
                Long = orderRouteEntity.Long,
                CreateDate = orderRouteEntity.CreateDate,
                OrderId = orderRouteEntity.OrderId,
            };
        }

        public static void ConvertModelToEntity(
            OrderRouteDTO orderRouteDTO,
            ref OrderRoute orderRouteEntity
        )
        {
            try
            {
                if (orderRouteDTO == null)
                {
                    LoggerHelper.Instance.LogError(
                        new ArgumentNullException(nameof(orderRouteDTO)),
                        "OrderRouteDTO is null"
                    );
                    throw new ArgumentNullException(
                        nameof(orderRouteDTO),
                        "Source orderRouteDTO cannot be null"
                    );
                }

                if (orderRouteEntity == null)
                {
                    LoggerHelper.Instance.LogError(
                        new ArgumentNullException(nameof(orderRouteEntity)),
                        "OrderRoute entity is null"
                    );
                    throw new ArgumentNullException(
                        nameof(orderRouteEntity),
                        "Target orderRouteEntity cannot be null"
                    );
                }

                orderRouteEntity.Id = orderRouteDTO.Id;
                orderRouteEntity.Lat = orderRouteDTO.Lat;
                orderRouteEntity.Long = orderRouteDTO.Long;
                orderRouteEntity.CreateDate = orderRouteDTO.CreateDate;
                orderRouteEntity.OrderId = orderRouteDTO.OrderId;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    "Error during OrderRouteDTO to OrderRoute entity conversion"
                );
                throw;
            }
        }
    }
}
