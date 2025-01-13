using System;
using System.ComponentModel.DataAnnotations;
using KiloTaxi.Common.Enums;

namespace KiloTaxi.Model.DTO.Request
{
    public class OrderFormDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "PickUpLocation is required.")]
        [StringLength(
            100,
            MinimumLength = 3,
            ErrorMessage = "PickUpLocation must be between 3 and 100 characters."
        )]
        public string PickUpLocation { get; set; }

        [Required(ErrorMessage = "PickUpLat is required.")]
        public string PickUpLat { get; set; }

        [Required(ErrorMessage = "PickUpLong is required.")]
        public string PickUpLong { get; set; }

        [Required(ErrorMessage = "DestinationLocation is required.")]
        [StringLength(
            100,
            MinimumLength = 3,
            ErrorMessage = "DestinationLocation must be between 3 and 100 characters."
        )]
        public string DestinationLocation { get; set; }

        [Required(ErrorMessage = "DestinationLat is required.")]
        public string DestinationLat { get; set; }

        [Required(ErrorMessage = "DestinationLong is required.")]
        public string DestinationLong { get; set; }

        public int? WalletTransactionId { get; set; }

        [Required(ErrorMessage = "CustomerId is required.")]
        public int CustomerId { get; set; }

        public int? DriverId { get; set; }

        public int? VehicleId { get; set; }

        public int? ScheduleBookingId { get; set; }

        [Required(ErrorMessage = "EstimatedAmount is required.")]
        [Range(0.01, 10000.00, ErrorMessage = "EstimatedAmount must be between 0.01 and 10,000.")]
        public decimal? EstimatedAmount { get; set; }

        public decimal? TotalAmount { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        public OrderStatus Status { get; set; }

        [Required(ErrorMessage = "CreatedDate is required.")]
        public DateTime CreatedDate { get; set; }

        public string? Notes { get; set; }
    }
}
