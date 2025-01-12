using System;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;

namespace KiloTaxi.Converter
{
    public static class OrderExtraDemandConverter
    {
        public static OrderExtraDemandDTO ConvertEntityToModel(OrderExtraDemand orderExtraDemandEntity)
        {
            if (orderExtraDemandEntity == null)
            {
                LoggerHelper.Instance.LogError(
                    new ArgumentNullException(nameof(orderExtraDemandEntity)),
                    "OrderExtraDemand entity is null"
                );
                throw new ArgumentNullException(
                    nameof(orderExtraDemandEntity),
                    "Source orderExtraDemandEntity cannot be null"
                );
            }

            return new OrderExtraDemandDTO()
            {
                Id = orderExtraDemandEntity.Id,
                OrderId = orderExtraDemandEntity.OrderId,
                ExtraDemandId = orderExtraDemandEntity.ExtraDemandId,
                Unit = orderExtraDemandEntity.Unit,
            };
        }

        public static void ConvertModelToEntity(
            OrderExtraDemandDTO orderExtraDemandDTO,
            ref OrderExtraDemand orderExtraDemandEntity
        )
        {
            try
            {
                if (orderExtraDemandDTO == null)
                {
                    LoggerHelper.Instance.LogError(
                        new ArgumentNullException(nameof(orderExtraDemandDTO)),
                        "OrderExtraDemandDTO is null"
                    );
                    throw new ArgumentNullException(
                        nameof(orderExtraDemandDTO),
                        "Source orderExtraDemandDTO cannot be null"
                    );
                }

                if (orderExtraDemandEntity == null)
                {
                    LoggerHelper.Instance.LogError(
                        new ArgumentNullException(nameof(orderExtraDemandEntity)),
                        "OrderExtraDemand entity is null"
                    );
                    throw new ArgumentNullException(
                        nameof(orderExtraDemandEntity),
                        "Target orderExtraDemandEntity cannot be null"
                    );
                }

                orderExtraDemandEntity.Id = orderExtraDemandDTO.Id;
                orderExtraDemandEntity.OrderId = orderExtraDemandDTO.OrderId;
                orderExtraDemandEntity.ExtraDemandId = orderExtraDemandDTO.ExtraDemandId;
                orderExtraDemandEntity.Unit = orderExtraDemandDTO.Unit;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    "Error during OrderExtraDemandDTO to OrderExtraDemand entity conversion"
                );
                throw;
            }
        }
    }
}
