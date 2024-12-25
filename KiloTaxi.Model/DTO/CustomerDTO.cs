using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KiloTaxi.Common.Enums;
using Microsoft.AspNetCore.Http;

namespace KiloTaxi.Model.DTO
{
    public class CustomerDTO
    {
        public int Id { get; set; }
        
        
        [MaxLength(100)]
        public string Name { get; set; }

        public string? Profile { get; set; }

        public string MobilePrefix { get; set; }

        public string Phone { get; set; }
        
        public string Role {get;set;}
        
        public string RefreshToken { get; set; }
        
        [DataType(DataType.Date)]
        public DateTime RefreshTokenExpiryTime { get; set; }
        
        [EmailAddress]
        public string Email { get; set; }
        
        [DataType(DataType.DateTime)]
        public DateTime? Dob { get; set; }
        
        public string Nrc { get; set; }

        
        public string? NrcImageFront { get; set; }
        
        public string? NrcImageBack { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? EmailVerifiedAt { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? PhoneVerifiedAt { get; set; }

        public string Password { get; set; }

        public GenderType Gender { get; set; }

        public string Address { get; set; }

        public string State { get; set; }

        public string City { get; set; }

        public string Township { get; set; }


        public CustomerStatus Status { get; set; }

        public KycStatus KycStatus { get; set; }
        
        public IFormFile? File_NrcImageFront { get; set; }
        public IFormFile? File_NrcImageBack { get; set; }
        public IFormFile? File_Profile { get; set; }
    }
}
