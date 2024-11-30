using System;
using KiloTaxi.Common.Enums;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;

namespace KiloTaxi.Converter
{
    public static class OrderConverter
    {
        public static OrderDTO ConvertEntityToModel(Order orderEntity)
        {
            if (orderEntity == null)
            {
                LoggerHelper.Instance.LogError(
                    new ArgumentNullException(nameof(orderEntity)),
                    "Order entity is null"
                );
                throw new ArgumentNullException(
                    nameof(orderEntity),
                    "Source orderEntity cannot be null"
                );
            }

            return new OrderDTO()
            {
                Id = orderEntity.Id,
                WalletTransactionId = orderEntity.WalletTransactionId,
                CustomerId = orderEntity.CustomerId,
                DriverId = orderEntity.DriverId,
                ScheduleBookingId = orderEntity.ScheduleBookingId,
                TotalAmount = orderEntity.TotalAmount,
                Status = Enum.Parse<OrderStatus>(orderEntity.Status),
                CreatedDate = orderEntity.CreatedDate,
            };
        }

        public static void ConvertModelToEntity(OrderDTO orderDTO, ref Order orderEntity)
        {
            try
            {
                if (orderDTO == null)
                {
                    LoggerHelper.Instance.LogError(
                        new ArgumentNullException(nameof(orderDTO)),
                        "OrderDTO is null"
                    );
                    throw new ArgumentNullException(
                        nameof(orderDTO),
                        "Source orderDTO cannot be null"
                    );
                }

                orderEntity.Id = orderDTO.Id;
                orderEntity.WalletTransactionId = orderDTO.WalletTransactionId;
                orderEntity.CustomerId = orderDTO.CustomerId;
                orderEntity.DriverId = orderDTO.DriverId;
                orderEntity.ScheduleBookingId = orderDTO.ScheduleBookingId;
                orderEntity.TotalAmount = orderDTO.TotalAmount;
                orderEntity.Status = orderDTO.Status.ToString();
                orderEntity.CreatedDate = orderDTO.CreatedDate;
            }
            catch (ArgumentException ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    "Argument exception during model-to-entity conversion"
                );
                throw;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    "Error during OrderDTO to Order entity conversion"
                );
                throw;
            }
        }
    }
}
