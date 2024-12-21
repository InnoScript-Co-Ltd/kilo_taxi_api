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
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string PromoCode { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; }

        public int? Quantity { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime ExpiredDate { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Unit { get; set; }

        [Required]
        public string PromotionType { get; set; }

        [Required]
        public string Status { get; set; }

        [Required]
        public string ApplicableTo { get; set; }

        public string? Description { get; set; }
        
        public ICollection<PromotionUser> PromotionUsers { get; set; }
    }
}
