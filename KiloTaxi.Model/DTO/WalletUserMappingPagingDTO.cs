using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiloTaxi.Model.DTO
{
    internal class WalletUserMappingPagingDTO
    {
        public PagingResult Paging { get; set; }
        public IEnumerable<WalletUserMappingDTO> WalletUserMappings { get; set; }
    }
}
