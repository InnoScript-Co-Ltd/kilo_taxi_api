using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KiloTaxi.Common.Enums;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Model.DTO.Response;

namespace KiloTaxi.Model.DTO
{
    public class OrderDTO
    {
        public int Id { get; set; }

        public string PickUpLocation { get; set; }

        public string PickUpLat { get; set; }

        public string PickUpLong { get; set; }

        public string DestinationLocation { get; set; }

        public string DestinationLat { get; set; }

        public string DestinationLong { get; set; }

        public int? WalletTransactionId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        public int? DriverId { get; set; }

        public int? VehicleId { get; set; }

        public int? ScheduleBookingId { get; set; }

        [Required]
        [Range(0.01, 10000.00)]
        public decimal? EstimatedAmount { get; set; }

        public decimal? TotalAmount { get; set; }

        public Customer? customer { get; set; }

        public Driver? driver { get; set; }

        [Required]
        public OrderStatus Status { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; }
    }
}
