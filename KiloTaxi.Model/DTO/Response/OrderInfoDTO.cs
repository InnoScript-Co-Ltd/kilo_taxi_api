using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using KiloTaxi.Common.Enums;

namespace KiloTaxi.Model.DTO.Response
{
    public class OrderInfoDTO
    {
        public int Id { get; set; }

        public string PickUpLocation { get; set; }
        public string PickUpLat { get; set; }
        public string PickUpLong { get; set; }

        public string DestinationLocation { get; set; }
        public string DestinationLat { get; set; }
        public string DestinationLong { get; set; }

        public int? WalletTransactionId { get; set; }

        public int CustomerId { get; set; }
        public CustomerInfoDTO Customer { get; set; }

        public int? DriverId { get; set; }
        public DriverInfoDTO Driver { get; set; }

        public int? VehicleId { get; set; }
        public IEnumerable<VehicleInfoDTO> VehicleInfo { get; set; }
        public List<OrderRouteInfoDTO>? OrderRouteInfo { get; set; }
        public int? ScheduleBookingId { get; set; }
        public ScheduleBookingDTO ScheduleBooking { get; set; }

        [Range(0.01, 10000.00)]
        public decimal? EstimatedAmount { get; set; }

        public decimal? TotalAmount { get; set; }

        public OrderStatus Status { get; set; }

        public DateTime CreatedDate { get; set; }

        public string? Notes { get; set; }
    }
}
