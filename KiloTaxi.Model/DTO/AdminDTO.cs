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

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public string Phone { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime EmailVerifiedAt { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime PhoneVerifiedAt { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public GenderType Gender { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string Status { get; set; }
    }
}
