using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KiloTaxi.Model.DTO;

namespace KiloTaxi.Model.DTO
{
    public class AdminPagingDTO
    {
        public PagingResult Paging { get; set; }
        public IEnumerable<AdminDTO> Admins { get; set; }
    }
}
