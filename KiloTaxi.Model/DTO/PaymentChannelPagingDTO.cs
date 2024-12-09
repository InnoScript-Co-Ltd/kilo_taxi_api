using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiloTaxi.Model.DTO
{
    public class PaymentChannelPagingDTO
    {
        public PagingResult Paging { get; set; }
        public IEnumerable<PaymentChannelDTO> PaymentChannels { get; set; }

    }
}
