using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KiloTaxi.Common.Enums;

namespace KiloTaxi.Model.DTO
{
    public class PromotionDTO
    {
        public int Id { get; set; }

        [Required]
        public string PromoCode { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; }

        public int? Quantity { get; set; }

        [Required]
        public DateTime ExpiredDate { get; set; }

        [Range(0.01, 10000.00)]
        public decimal Unit { get; set; }

        [Required]
        public PromotionType PromotionType { get; set; }

        [Required]
        public ApplicableTo ApplicableTo { get; set; }

        [Required]
        public PromotionStatus Status { get; set; }

        public string? Description { get; set; }
    }
}
