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
    public class Admin
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        

        [MaxLength(100)]
        public string Name { get; set; }

        public string Phone { get; set; }

        [Required]
        public string Email { get; set; }
        
        public DateTime? EmailVerifiedAt { get; set; }
        
        public DateTime? PhoneVerifiedAt { get; set; }
        
        public string Role { get; set; }
        public string Password { get; set; }

        public string Gender { get; set; }

        public string Address { get; set; }

        public string Status { get; set; }
        
        public string RefreshToken { get; set; }
        
        [DataType(DataType.Date)]
        public DateTime RefreshTokenExpiryTime { get; set; }
        
    }
}
