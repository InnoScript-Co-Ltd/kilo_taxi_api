using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiloTaxi.Model.DTO
{
    public class TripLocation
    {
        public string OrderId { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }
    }
}
