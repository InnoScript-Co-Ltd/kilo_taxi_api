using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiloTaxi.EntityFramework.EntityModel
{
    public class Customer
    {   
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public string? Profile { get; set; }

        [Required]
        public string MobilePrefix { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string Email { get; set; }
        public DateTime? Dob { get; set; }
        public string? Nrc { get; set; }
        public string? NrcImageFront { get; set; }
        public string? NrcImageBack { get; set; }
        
        public DateTime? EmailVerifiedAt { get; set; }
        
        public DateTime? PhoneVerifiedAt { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Township { get; set; }

        [Required]
        public string Status { get; set; }

        [Required]
        public string KycStatus { get; set; }
        
        public ICollection<PromotionUser> PromotionUsers { get; set; }

    }
}
