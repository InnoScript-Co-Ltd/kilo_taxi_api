using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiloTaxi.Model.DTO
{
    public class VehicleLocation
    {
        public string VehicleId { get; set; }
        public string Lat { get; set; }
        public string? Long { get; set; }
        public string? TimeStamp {get;set;} 
        public double?  Accuracy { get; set; }
        public double? Altitude {get;set;}
        public double? AltitudeAccuracy{get;set;}
        public double? Heading{get;set;}
        public double?  HeadingAccuracy{get;set;}
        public double? Speed{get;set;}
        public double? SpeedAccuracy{get;set;}
    }
}
