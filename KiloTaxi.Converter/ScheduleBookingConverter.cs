using System;
using KiloTaxi.Common.Enums;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;

namespace KiloTaxi.Converter
{
    public static class ScheduleBookingConverter
    {
        public static ScheduleBookingInfoDTO ConvertEntityToModel(ScheduleBooking scheduleBookingEntity)
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

            return new ScheduleBookingInfoDTO()
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

        public static void ConvertModelToEntity(ScheduleBookingFormDTO scheduleBookingFormDTO, ref ScheduleBooking scheduleBookingEntity)
        {
            try
            {
                if (scheduleBookingFormDTO == null)
                {
                    LoggerHelper.Instance.LogError(
                        new ArgumentNullException(nameof(scheduleBookingFormDTO)),
                        "ScheduleBookingFormDTO is null"
                    );
                    throw new ArgumentNullException(
                        nameof(scheduleBookingFormDTO),
                        "Source scheduleBookingFormDTO cannot be null"
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

                scheduleBookingEntity.Id = scheduleBookingFormDTO.Id;
                scheduleBookingEntity.CustomerId = scheduleBookingFormDTO.CustomerId;
                scheduleBookingEntity.DriverId = scheduleBookingFormDTO.DriverId;
                scheduleBookingEntity.PickUpLocation = scheduleBookingFormDTO.PickUpLocation;
                scheduleBookingEntity.PickUpLat = scheduleBookingFormDTO.PickUpLat;
                scheduleBookingEntity.PickUpLong = scheduleBookingFormDTO.PickUpLong;
                scheduleBookingEntity.DestinationLocation = scheduleBookingFormDTO.DestinationLocation;
                scheduleBookingEntity.DestinationLat = scheduleBookingFormDTO.DestinationLat;
                scheduleBookingEntity.DestinationLong = scheduleBookingFormDTO.DestinationLong;
                scheduleBookingEntity.ScheduleTime = scheduleBookingFormDTO.ScheduleTime;
                scheduleBookingEntity.Status = scheduleBookingFormDTO.Status.ToString();
                scheduleBookingEntity.CreatedDate = scheduleBookingFormDTO.CreatedDate;
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
