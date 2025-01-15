using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KiloTaxi.Common.Enums;

namespace KiloTaxi.Model.DTO.Response
{
    public class PromotionInfoDTO
    {
        public int Id { get; set; }

        public string PromoCode { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? Quantity { get; set; }

        public DateTime ExpiredDate { get; set; }

        public decimal Unit { get; set; }

        public PromotionType PromotionType { get; set; }

        public ApplicableTo ApplicableTo { get; set; }

        public PromotionStatus Status { get; set; }

        public string? Description { get; set; }

        // Input: List of customer IDs (for creation or update)
        public List<int>? CustomerIds { get; set; }

        // Output: List of customer names (for display)
        public List<string>? CustomerNames { get; set; }
    }
}
