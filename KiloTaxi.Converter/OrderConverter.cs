using System;
using KiloTaxi.Common.ConfigurationSettings;
using KiloTaxi.Common.Enums;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;
using Microsoft.Extensions.Options;

namespace KiloTaxi.Converter
{
    public static class OrderConverter
    {
        private static string _mediaHostUrl="http://localhost/kilotaxi.media/";
        
        public static OrderInfoDTO ConvertEntityToModel(Order orderEntity)
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

            return new OrderInfoDTO()
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
                Customer = orderEntity.Customer != null
                    ? CustomerConverter.ConvertEntityToModel(orderEntity.Customer,_mediaHostUrl)
                    : null,
                Driver = orderEntity.Driver != null
                    ? DriverConverter.ConvertEntityToModel(orderEntity.Driver,_mediaHostUrl)
                    : null,
                ScheduleBookingId = orderEntity.ScheduleBookingId,
                EstimatedAmount = orderEntity.EstimatedAmount,
                Status = Enum.Parse<OrderStatus>(orderEntity.Status),
                CreatedDate = orderEntity.CreatedDate,
            };
        }

        public static void ConvertModelToEntity(OrderFormDTO orderFormDTO, ref Order orderEntity)
        {
            try
            {
                if (orderFormDTO == null)
                {
                    LoggerHelper.Instance.LogError(
                        new ArgumentNullException(nameof(orderFormDTO)),
                        "OrderDTO is null"
                    );
                    throw new ArgumentNullException(
                        nameof(orderFormDTO),
                        "Source orderFormDTO cannot be null"
                    );
                }

                orderEntity.Id = orderFormDTO.Id;
                orderEntity.PickUpLocation = orderFormDTO.PickUpLocation;
                orderEntity.PickUpLat = orderFormDTO.PickUpLat;
                orderEntity.PickUpLong = orderFormDTO.PickUpLong;
                orderEntity.DestinationLocation = orderFormDTO.DestinationLocation;
                orderEntity.DestinationLat = orderFormDTO.DestinationLat;
                orderEntity.DestinationLong = orderFormDTO.DestinationLong;
                orderEntity.WalletTransactionId = orderFormDTO.WalletTransactionId;
                orderEntity.CustomerId = orderFormDTO.CustomerId;
                orderEntity.DriverId = orderFormDTO.DriverId;
                orderEntity.VehicleId = orderFormDTO.VehicleId;
                orderEntity.ScheduleBookingId = orderFormDTO.ScheduleBookingId;
                orderEntity.EstimatedAmount = orderFormDTO.EstimatedAmount;
                orderEntity.Status = orderFormDTO.Status.ToString();
                orderEntity.CreatedDate = orderFormDTO.CreatedDate;
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
