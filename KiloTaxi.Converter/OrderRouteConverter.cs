using System;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;

namespace KiloTaxi.Converter
{
    public static class OrderRouteConverter
    {
        public static OrderRouteInfoDTO ConvertEntityToModel(OrderRoute orderRouteEntity)
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

            return new OrderRouteInfoDTO()
            {
                Id = orderRouteEntity.Id,
                Lat = orderRouteEntity.Lat,
                Long = orderRouteEntity.Long,
                CreateDate = orderRouteEntity.CreateDate,
                OrderId = orderRouteEntity.OrderId,
            };
        }

        public static void ConvertModelToEntity(
            OrderRouteFormDTO orderRouteFormDTO,
            ref OrderRoute orderRouteEntity
        )
        {
            try
            {
                if (orderRouteFormDTO == null)
                {
                    LoggerHelper.Instance.LogError(
                        new ArgumentNullException(nameof(orderRouteFormDTO)),
                        "OrderRouteFormDTO is null"
                    );
                    throw new ArgumentNullException(
                        nameof(orderRouteFormDTO),
                        "Source orderRouteFormDTO cannot be null"
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

                orderRouteEntity.Id = orderRouteFormDTO.Id;
                orderRouteEntity.Lat = orderRouteFormDTO.Lat;
                orderRouteEntity.Long = orderRouteFormDTO.Long;
                orderRouteEntity.CreateDate = orderRouteFormDTO.CreateDate;
                orderRouteEntity.OrderId = orderRouteFormDTO.OrderId;
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
