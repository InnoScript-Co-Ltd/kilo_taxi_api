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
        public Decimal TotalAmount { get; set; }
        
        [Required]
        public OrderStatus Status { get; set; }
        
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime createdDate { get; set; }
        
        [ForeignKey("WalletTransaction")]
        public int? WalletTransactionId { get; set; }
        public virtual WalletTransaction WalletTransaction { get; set; }
        
        [ForeignKey("Customer")]
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
        
        [ForeignKey("Driver")]
        public int DriverId { get; set; }
        public virtual Driver Driver { get; set; }
        
        [ForeignKey("ScheduleBooking")]
        public int? ScheduleBookingId { get; set; }
        public virtual ScheduleBooking ScheduleBooking { get; set; }
        
    }

}