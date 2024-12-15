using KiloTaxi.Model.DTO;

namespace KiloTaxi.DataAccess.Interface;

public interface IAuditLogRepository
{
    AuditLogPagingDTO GetAllAuditLog(PageSortParam pageSortParam);
    AuditLogDTO CreateAuditLog(AuditLogDTO auditLogDTO);
    AuditLogDTO GetAuditLogByID(int id);
}
