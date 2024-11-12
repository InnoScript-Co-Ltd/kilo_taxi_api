using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KiloTaxi.Model.DTO;

namespace KiloTaxi.Model.DTO
{
    public class CustomerPagingDTO
    {
        public PagingResult Paging { get; set; }
        public IEnumerable<CustomerDTO> Customers { get; set; }
    }
}
