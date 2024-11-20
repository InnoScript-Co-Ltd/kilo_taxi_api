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

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime? CheckinTime { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime? CheckoutTime { get; set; }

        [Required]
        [Range(0.01, 10000.00)]
        public decimal Amount { get; set; }

        [Required]
        public string PaymentType { get; set; }

        [Required]
        public string OrderStatus { get; set; }


        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int DriverId { get; set; }

        [AllowNull]
        public int WalletFromId { get; set; }

        [AllowNull]
        public int WalletToId { get; set; }

        [AllowNull]
        public int PaymentChannelId { get; set; }


    }
}