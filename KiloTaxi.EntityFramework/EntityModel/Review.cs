using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiloTaxi.EntityFramework.EntityModel
{
    public class Review
    {
        public int Id { get; set; }

        [Required]
        public int Rating { get; set; }

        public string ReviewContent { get; set; } //Can't use Review Because it is same as its enclosing type

        [ForeignKey("Customer")]
        public int CustomerId { get; set; }

        [ForeignKey("Driver")]
        public int DriverId { get; set; }
    }
}
