using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace KiloTaxi.EntityFramework.EntityModel
{
    [Index(nameof(Phone), IsUnique = true)]
    [Index(nameof(Email), IsUnique = true)] 
    public class Customer
    {   
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [MaxLength(100)]
        public string Name { get; set; }

        public string? Profile { get; set; }

        public string? MobilePrefix { get; set; }

        public string Phone { get; set; }
        
        public string? Email { get; set; }
        public DateTime? Dob { get; set; }
        public string? Nrc { get; set; }
        public string? NrcImageFront { get; set; }
        public string? NrcImageBack { get; set; }
        
        public DateTime? EmailVerifiedAt { get; set; }
        
        public DateTime? PhoneVerifiedAt { get; set; }
        
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; }

        public string? Password { get; set; }

        public string? Gender { get; set; }

        public string? Address { get; set; }

        public string? State { get; set; }

        public string? City { get; set; }

        public string? Township { get; set; }

        public string? Status { get; set; }

        public string? KycStatus { get; set; }
        
        public string Role { get; set; }
        
        public string? RefreshToken { get; set; }
        
        public string? Otp {get;set;}

        public DateTime? RefreshTokenExpiryTime { get; set; }
        
        public ICollection<PromotionUser> PromotionUsers { get; set; }

    }
}
