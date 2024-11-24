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

        [Required]
        public DateTime ExpiredAt { get; set; }

        [Range(0.01, 10000.00)]
        public decimal? FixAmount { get; set; }

        public int? Percentage { get; set; }

        [Required]
        public PromotionStatus Status { get; set; }

        public int CustomerId { get; set; }
        public string? CustomerName { get; set; }
    }
}
