using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiloTaxi.EntityFramework.EntityModel
{
    public class Admin
    {   
        public int Id { get; set; }
        [Required] 
        [MaxLength(100)]
        public string Name { get; set; }
        [Required]
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime EmailVerifiedAt { get; set; }
        public DateTime PhoneVerifiedAt { get; set; }
        public string Password { get; set; }
        [Required]
        public string Gender { get; set; }
        public string Address { get; set; }
        public string Status { get; set; }
    }
}
