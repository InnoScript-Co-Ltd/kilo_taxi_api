using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KiloTaxi.Common.Enums;

namespace KiloTaxi.Model.DTO.Request
{
    public class OrderRouteFormDTO
    {
        public int Id { get; set; }

        [Required]
        public string Lat { get; set; }
        public string Long { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreateDate { get; set; }
        public int OrderId { get; set; }
    }
}
