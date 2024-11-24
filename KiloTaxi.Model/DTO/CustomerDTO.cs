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

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public string? Profile { get; set; }

        [Required]
        public string MobilePrefix { get; set; }

        [Required]
        public string Phone { get; set; }

        [EmailAddress]
        public string Email { get; set; }
        
        [DataType(DataType.DateTime)]
        public DateTime? Dob { get; set; }
        
        [Required]
        public string Nrc { get; set; }

        
        public string? NrcImageFront { get; set; }
        
        public string? NrcImageBack { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? EmailVerifiedAt { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? PhoneVerifiedAt { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public GenderType Gender { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Township { get; set; }


        [Required]
        public CustomerStatus Status { get; set; }

        [Required]
        public KycStatus KycStatus { get; set; }
        
        public IFormFile? File_NrcImageFront { get; set; }
        public IFormFile? File_NrcImageBack { get; set; }
        public IFormFile? File_Profile { get; set; }
    }
}
