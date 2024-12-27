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
                PickUpLocation = orderEntity.PickUpLocation,
                PickUpLat = orderEntity.PickUpLat,
                PickUpLong = orderEntity.PickUpLong,
                DestinationLocation = orderEntity.DestinationLocation,
                DestinationLat = orderEntity.DestinationLat,
                DestinationLong = orderEntity.DestinationLong,
                WalletTransactionId = orderEntity.WalletTransactionId,
                CustomerId = orderEntity.CustomerId,
                DriverId = orderEntity.DriverId,
                VehicleId = orderEntity.VehicleId,
                ScheduleBookingId = orderEntity.ScheduleBookingId,
                EstimatedAmount = orderEntity.EstimatedAmount,
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
                orderEntity.PickUpLocation = orderDTO.PickUpLocation;
                orderEntity.PickUpLat = orderDTO.PickUpLat;
                orderEntity.PickUpLong = orderDTO.PickUpLong;
                orderEntity.DestinationLocation = orderDTO.DestinationLocation;
                orderEntity.DestinationLat = orderDTO.DestinationLat;
                orderEntity.DestinationLong = orderDTO.DestinationLong;
                orderEntity.WalletTransactionId = orderDTO.WalletTransactionId;
                orderEntity.CustomerId = orderDTO.CustomerId;
                orderEntity.DriverId = orderDTO.DriverId;
                orderEntity.VehicleId = orderDTO.VehicleId;
                orderEntity.ScheduleBookingId = orderDTO.ScheduleBookingId;
                orderEntity.EstimatedAmount = orderDTO.EstimatedAmount;
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
