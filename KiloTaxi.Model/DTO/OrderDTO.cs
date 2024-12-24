using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KiloTaxi.Common.Enums;

namespace KiloTaxi.Model.DTO
{
    public class OrderDTO
    {
        public int Id { get; set; }
        
        public string PickUpLocation {get; set;}
        
        public string PickUpLat { get; set; }
        
        public string PickUpLong  {get; set;}
        
        public string DestinationLocation {get; set;}
        
        public string DestinationLat{get; set;}
        
        public string DestinationLong {get; set;}

        public int? WalletTransactionId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int DriverId { get; set; }

        [AllowNull]
        public int? ScheduleBookingId { get; set; }

        [Required]
        [Range(0.01, 10000.00)]
        public decimal EstimatedAmount { get; set; }

        [Required]
        public OrderStatus Status { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; }
    }
}
