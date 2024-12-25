using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KiloTaxi.Common.Enums;

namespace KiloTaxi.Model.DTO
{
    public class AdminDTO
    {
        public int Id { get; set; }
        
        [MaxLength(100)]
        public string? Name { get; set; }

        public string? Phone { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? EmailVerifiedAt { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? PhoneVerifiedAt { get; set; }

        public string Password { get; set; }
        
        public string? Role { get; set; }
        
        public string RefreshToken { get; set; }
        
        [DataType(DataType.Date)]
        public DateTime RefreshTokenExpiryTime { get; set; } 

        public GenderType Gender { get; set; }

        public string? Address { get; set; }

        public CustomerStatus Status { get; set; }
        
        
    }
}
