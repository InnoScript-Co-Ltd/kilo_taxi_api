using System.Linq.Expressions;
using KiloTaxi.Converter;
using KiloTaxi.DataAccess.Interface;
using KiloTaxi.EntityFramework;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using Microsoft.EntityFrameworkCore;

namespace KiloTaxi.DataAccess.Implementation
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly DbKiloTaxiContext _dbKiloTaxiContext;

        public AuditLogRepository(DbKiloTaxiContext dbContext)
        {
            _dbKiloTaxiContext = dbContext;
        }

        public AuditLogPagingDTO GetAllAuditLog(PageSortParam pageSortParam)
        {
            try
            {
                var query = _dbKiloTaxiContext.AuditLogs.AsQueryable();

                int totalCount = query.Count();

                if (!string.IsNullOrEmpty(pageSortParam.SortField))
                {
                    var param = Expression.Parameter(typeof(AuditLog), "auditLog");
                    var property = Expression.Property(param, pageSortParam.SortField);
                    var sortExpression = Expression.Lambda(property, param);

                    string sortMethod =
                        pageSortParam.SortDir == SortDirection.ASC
                            ? "OrderBy"
                            : "OrderByDescending";
                    var orderByMethod = typeof(Queryable)
                        .GetMethods()
                        .Where(m => m.Name == sortMethod && m.GetParameters().Length == 2)
                        .Single()
                        .MakeGenericMethod(typeof(AuditLog), property.Type);

                    query =
                        (IQueryable<AuditLog>)
                            orderByMethod.Invoke(null, new object[] { query, sortExpression });
                }

                if (query.Count() > pageSortParam.PageSize)
                {
                    query = query
                        .Skip((pageSortParam.CurrentPage - 1) * pageSortParam.PageSize)
                        .Take(pageSortParam.PageSize);
                }

                var auditLog = query
                    .Select(AuditLogConverter.ConvertEntityToModel)
                    .ToList();

                var totalPages = (int)Math.Ceiling((double)totalCount / pageSortParam.PageSize);
                var pagingResult = new PagingResult
                {
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    PreviousPage =
                        pageSortParam.CurrentPage > 1 ? pageSortParam.CurrentPage - 1 : (int?)null,
                    NextPage =
                        pageSortParam.CurrentPage < totalPages
                            ? pageSortParam.CurrentPage + 1
                            : (int?)null,
                    FirstRowOnPage = ((pageSortParam.CurrentPage - 1) * pageSortParam.PageSize) + 1,
                    LastRowOnPage = Math.Min(
                        totalCount,
                        pageSortParam.CurrentPage * pageSortParam.PageSize
                    ),
                };

                return new AuditLogPagingDTO
                {
                    Paging = pagingResult,
                    AuditLogs = auditLog,
                };
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    "Error occurred while fetching all audit log."
                );
                throw;
            }
        }

        public AuditLogDTO CreateAuditLog(AuditLogDTO auditLogDTO)
        {
            try
            {
                AuditLog auditLogEntity = new AuditLog();
                AuditLogConverter.ConvertModelToEntity(
                    auditLogDTO,
                    ref auditLogEntity
                );

                _dbKiloTaxiContext.Add(auditLogEntity);
                _dbKiloTaxiContext.SaveChanges();

                auditLogDTO.Id = auditLogEntity.Id;

                LoggerHelper.Instance.LogInfo(
                    $"Audit Log created successfully with Id: {auditLogEntity.Id}"
                );

                return auditLogDTO;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(ex, "Error occurred while adding Audit Log.");
                throw;
            }
        }

        public AuditLogDTO GetAuditLogByID(int id)
        {
            try
            {
                var auditLogEntity = _dbKiloTaxiContext.AuditLogs.FirstOrDefault(log =>
                    log.Id == id
                );

                if (auditLogEntity == null)
                {
                    LoggerHelper.Instance.LogError($"Audit Log with Id: {id} not found.");
                    return null;
                }

                return AuditLogConverter.ConvertEntityToModel(auditLogEntity);
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    $"Error occurred while fetching audit log with Id: {id}"
                );
                throw;
            }
        }
    }
}
