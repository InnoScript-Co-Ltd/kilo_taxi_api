using System;
using KiloTaxi.Common.Enums;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;

namespace KiloTaxi.Converter
{
    public static class ScheduleBookingConverter
    {
        public static ScheduleBookingDTO ConvertEntityToModel(ScheduleBooking scheduleBookingEntity)
        {
            if (scheduleBookingEntity == null)
            {
                LoggerHelper.Instance.LogError(
                    new ArgumentNullException(nameof(scheduleBookingEntity)),
                    "ScheduleBooking entity is null"
                );
                throw new ArgumentNullException(
                    nameof(scheduleBookingEntity),
                    "Source scheduleBookingEntity cannot be null"
                );
            }

            return new ScheduleBookingDTO()
            {
                Id = scheduleBookingEntity.Id,
                CustomerId = scheduleBookingEntity.CustomerId,
                DriverId = scheduleBookingEntity.DriverId,
                PickUpLocation = scheduleBookingEntity.PickUpLocation,
                PickUpLat = scheduleBookingEntity.PickUpLat,
                PickUpLong = scheduleBookingEntity.PickUpLong,
                DestinationLocation = scheduleBookingEntity.DestinationLocation,
                DestinationLat = scheduleBookingEntity.DestinationLat,
                DestinationLong = scheduleBookingEntity.DestinationLong,
                ScheduleTime = scheduleBookingEntity.ScheduleTime,
                Status =Enum.Parse<ScheduleStatus>(scheduleBookingEntity.Status),
                CreatedDate = scheduleBookingEntity.CreatedDate,
            };
        }

        public static void ConvertModelToEntity(ScheduleBookingDTO scheduleBookingDTO, ref ScheduleBooking scheduleBookingEntity)
        {
            try
            {
                if (scheduleBookingDTO == null)
                {
                    LoggerHelper.Instance.LogError(
                        new ArgumentNullException(nameof(scheduleBookingDTO)),
                        "ScheduleBookingDTO is null"
                    );
                    throw new ArgumentNullException(
                        nameof(scheduleBookingDTO),
                        "Source scheduleBookingDTO cannot be null"
                    );
                }

                if (scheduleBookingEntity == null)
                {
                    LoggerHelper.Instance.LogError(
                        new ArgumentNullException(nameof(scheduleBookingEntity)),
                        "ScheduleBooking entity is null"
                    );
                    throw new ArgumentNullException(
                        nameof(scheduleBookingEntity),
                        "Target scheduleBookingEntity cannot be null"
                    );
                }

                scheduleBookingEntity.Id = scheduleBookingDTO.Id;
                scheduleBookingEntity.CustomerId = scheduleBookingDTO.CustomerId;
                scheduleBookingEntity.DriverId = scheduleBookingDTO.DriverId;
                scheduleBookingEntity.PickUpLocation = scheduleBookingDTO.PickUpLocation;
                scheduleBookingEntity.PickUpLat = scheduleBookingDTO.PickUpLat;
                scheduleBookingEntity.PickUpLong = scheduleBookingDTO.PickUpLong;
                scheduleBookingEntity.DestinationLocation = scheduleBookingDTO.DestinationLocation;
                scheduleBookingEntity.DestinationLat = scheduleBookingDTO.DestinationLat;
                scheduleBookingEntity.DestinationLong = scheduleBookingDTO.DestinationLong;
                scheduleBookingEntity.ScheduleTime = scheduleBookingDTO.ScheduleTime;
                scheduleBookingEntity.Status = scheduleBookingDTO.Status.ToString();
                scheduleBookingEntity.CreatedDate = scheduleBookingDTO.CreatedDate;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    "Error during ScheduleBookingDTO to ScheduleBooking entity conversion"
                );
                throw;
            }
        }
    }
}
