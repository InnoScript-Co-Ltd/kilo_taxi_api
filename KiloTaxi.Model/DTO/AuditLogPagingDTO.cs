using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiloTaxi.Model.DTO
{
    public class AuditLogPagingDTO
    {
        public PagingResult Paging { get; set; }
        public IEnumerable<AuditLogDTO> AuditLogs { get; set; }
    }
}
