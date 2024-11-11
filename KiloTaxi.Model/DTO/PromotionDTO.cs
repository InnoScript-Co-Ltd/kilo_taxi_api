using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiloTaxi.Model.DTO
{
    public class PromotionDTO
    {
        public int Id { get; set; }

        [Required]
        public string PromoCode { get; set; }

        [Required]
        public DateTime ExpiredAt { get; set; }

        [Range(0, 9999999.99)]
        public float FixAmount { get; set; }

        public int Percentage { get; set; }

        [Required]
        public string Status { get; set; }

        public int CustomerId { get; set; }
        public string? CustomerName { get; set; }
    }
}
