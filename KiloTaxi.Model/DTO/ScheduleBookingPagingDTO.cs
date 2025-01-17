using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Response;

namespace KiloTaxi.Model.DTO
{
    public class ScheduleBookingPagingDTO
    {
        public PagingResult Paging { get; set; }
        public IEnumerable<ScheduleBookingInfoDTO> ScheduleBookings { get; set; }
    }
}
