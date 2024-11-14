using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiloTaxi.EntityFramework.EntityModel
{
    public class Promotion
    {
        public int Id { get; set; }

        [Required]
        public string PromoCode { get; set; }

        [Required]
        public DateTime ExpiredAt { get; set; }

        public float FixAmount { get; set; }
        public int Percentage { get; set; }

        [Required]
        public string Status { get; set; }

        [ForeignKey("Customer")]
        public int CustomerId { get; set; }
    }
}