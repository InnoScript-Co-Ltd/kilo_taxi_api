using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiloTaxi.Model.DTO.Response
{
    public class ReviewInfoDTO
    {
        public int Id { get; set; }

        public int Rating { get; set; }

        public string? ReviewContent { get; set; }
        public int CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public int DriverId { get; set; }
        public string? DriverName { get; set; }
    }
}
