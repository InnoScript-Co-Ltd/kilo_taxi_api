using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KiloTaxi.Common.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace KiloTaxi.EntityFramework.EntityModel {

    public class Order {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
         
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public Decimal? EstimatedAmount { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public Decimal? TotalAmount { get; set; }
        
        [Required]
        public string Status { get; set; }
        
        public string PickUpLocation {get; set;}
        
        public string PickUpLat { get; set; }
        
        public string PickUpLong  {get; set;}
        
        public string DestinationLocation {get; set;}
        
        public string DestinationLat{get; set;}
        
        public string DestinationLong {get; set;}
        
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; }
        
        [ForeignKey("WalletTransaction")]
        public int? WalletTransactionId { get; set; }
        public virtual WalletTransaction WalletTransaction { get; set; }
        
        [ForeignKey("Customer")]
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
        
        [ForeignKey("Driver")]
        public int? DriverId { get; set; }
        public virtual Driver Driver { get; set; }
        
        [ForeignKey("Vehicle")]
        public int? VehicleId { get; set; }
        public virtual Vehicle Vehicle { get; set; }
        
        [ForeignKey("ScheduleBooking")]
        public int? ScheduleBookingId { get; set; }
        public virtual ScheduleBooking ScheduleBooking { get; set; }
        
        public ICollection<OrderExtraDemand> OrderExtraDemand { get; set; }

    }

}