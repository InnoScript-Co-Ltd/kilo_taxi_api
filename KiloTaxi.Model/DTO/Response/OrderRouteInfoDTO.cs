using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KiloTaxi.Common.Enums;

namespace KiloTaxi.Model.DTO.Response
{
    public class OrderRouteInfoDTO
    {
        public int Id { get; set; }

        public string Lat { get; set; }
        public string Long { get; set; }

        public DateTime CreateDate { get; set; }
        public int OrderId { get; set; }
    }
}
