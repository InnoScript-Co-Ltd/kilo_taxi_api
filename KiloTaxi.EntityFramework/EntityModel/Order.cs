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
        
        public DateTime? CheckinTime { get; set; }
        
        public DateTime? CheckoutTime { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        public string PaymentType { get; set; }

        public string OrderStatus { get; set; }

        [ForeignKey("Customer")]
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        [ForeignKey("Driver")]
        public int DriverId { get; set; }
        public virtual Driver Driver { get; set; }

        [ForeignKey("Wallet")]
        public int WalletFromId { get; set; }
        public virtual Wallet WalletFrom { get; set; }


        [ForeignKey("Wallet")]
        public int WalletToId { get; set; }
        public virtual Wallet WalletTo { get; set; }

        [ForeignKey("PaymentChannel")]
        public int PaymentChannelId { get; set; }
        public virtual PaymentChannel PaymentChannel { get; set; }

    }

}