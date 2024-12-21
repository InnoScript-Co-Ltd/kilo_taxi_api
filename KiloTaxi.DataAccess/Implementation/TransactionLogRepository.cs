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
    public class TransactionLogRepository : ITransactionLogRepository
    {
        private readonly DbKiloTaxiContext _dbKiloTaxiContext;

        public TransactionLogRepository(DbKiloTaxiContext dbContext)
        {
            _dbKiloTaxiContext = dbContext;
        }

        public TransactionLogPagingDTO GetAllTransactionLog(PageSortParam pageSortParam)
        {
            try
            {
                var query = _dbKiloTaxiContext.TransactionLogs.AsQueryable();

                if (!string.IsNullOrEmpty(pageSortParam.SearchTerm))
                {
                    query = query.Where(log =>
                        log.OperationType.Contains(pageSortParam.SearchTerm)
                        || log.Details.Contains(pageSortParam.SearchTerm)
                        || log.PerformedBy.Contains(pageSortParam.SearchTerm)
                    );
                }

                int totalCount = query.Count();

                if (!string.IsNullOrEmpty(pageSortParam.SortField))
                {
                    var param = Expression.Parameter(typeof(TransactionLog), "transactionLog");
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
                        .MakeGenericMethod(typeof(TransactionLog), property.Type);

                    query =
                        (IQueryable<TransactionLog>)
                            orderByMethod.Invoke(null, new object[] { query, sortExpression });
                }

                if (query.Count() > pageSortParam.PageSize)
                {
                    query = query
                        .Skip((pageSortParam.CurrentPage - 1) * pageSortParam.PageSize)
                        .Take(pageSortParam.PageSize);
                }

                var transactionLog = query
                    .Select(TransactionLogConverter.ConvertEntityToModel)
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

                return new TransactionLogPagingDTO
                {
                    Paging = pagingResult,
                    TransactionLogs = transactionLog,
                };
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    "Error occurred while fetching all transaction log."
                );
                throw;
            }
        }

        public TransactionLogDTO CreateTransactionLog(TransactionLogDTO transactionLogDTO)
        {
            try
            {
                TransactionLog transactionLogEntity = new TransactionLog();
                TransactionLogConverter.ConvertModelToEntity(
                    transactionLogDTO,
                    ref transactionLogEntity
                );

                _dbKiloTaxiContext.Add(transactionLogEntity);
                _dbKiloTaxiContext.SaveChanges();

                transactionLogDTO.Id = transactionLogEntity.Id;

                LoggerHelper.Instance.LogInfo(
                    $"Transaction Log created successfully with Id: {transactionLogEntity.Id}"
                );

                return transactionLogDTO;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(ex, "Error occurred while adding Transaction Log.");
                throw;
            }
        }

        public TransactionLogDTO GetTransactionLogByID(int id)
        {
            try
            {
                var transactionLogEntity = _dbKiloTaxiContext.TransactionLogs.FirstOrDefault(log =>
                    log.Id == id
                );

                if (transactionLogEntity == null)
                {
                    LoggerHelper.Instance.LogError($"Transaction Log with Id: {id} not found.");
                    return null;
                }

                return TransactionLogConverter.ConvertEntityToModel(transactionLogEntity);
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    $"Error occurred while fetching transaction log with Id: {id}"
                );
                throw;
            }
        }
    }
}
