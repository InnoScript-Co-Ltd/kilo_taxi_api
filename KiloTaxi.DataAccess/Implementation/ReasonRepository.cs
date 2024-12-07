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
    public class ReasonRepository : IReasonRepository
    {
        private readonly DbKiloTaxiContext _dbKiloTaxiContext;

        public ReasonRepository(DbKiloTaxiContext dbContext)
        {
            _dbKiloTaxiContext = dbContext;
        }

        public ReasonPagingDTO GetAllReason(PageSortParam pageSortParam)
        {
            try
            {
                var query = _dbKiloTaxiContext.Reasons.AsQueryable();

                if (!string.IsNullOrEmpty(pageSortParam.SearchTerm))
                {
                    query = query.Where(reason => reason.Name.Contains(pageSortParam.SearchTerm));
                }

                int totalCount = query.Count();

                if (!string.IsNullOrEmpty(pageSortParam.SortField))
                {
                    var param = Expression.Parameter(typeof(Reason), "reason");
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
                        .MakeGenericMethod(typeof(Reason), property.Type);

                    query =
                        (IQueryable<Reason>)
                            orderByMethod.Invoke(null, new object[] { query, sortExpression });
                }

                if (query.Count() > pageSortParam.PageSize)
                {
                    query = query
                        .Skip((pageSortParam.CurrentPage - 1) * pageSortParam.PageSize)
                        .Take(pageSortParam.PageSize);
                }

                var reasons = query.Select(ReasonConverter.ConvertEntityToModel).ToList();

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

                return new ReasonPagingDTO { Paging = pagingResult, Reasons = reasons };
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(ex, "Error occurred while fetching all reasons.");
                throw;
            }
        }

        public ReasonDTO CreateReason(ReasonDTO reasonDTO)
        {
            try
            {
                Reason reasonEntity = new Reason();
                ReasonConverter.ConvertModelToEntity(reasonDTO, ref reasonEntity);

                _dbKiloTaxiContext.Add(reasonEntity);
                _dbKiloTaxiContext.SaveChanges();

                reasonDTO.Id = reasonEntity.Id;

                LoggerHelper.Instance.LogInfo(
                    $"Reason added successfully with Id: {reasonEntity.Id}"
                );

                return reasonDTO;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(ex, "Error occurred while adding Reason.");
                throw;
            }
        }

        public bool UpdateReason(ReasonDTO reasonDTO)
        {
            try
            {
                var reasonEntity = _dbKiloTaxiContext.Reasons.FirstOrDefault(r =>
                    r.Id == reasonDTO.Id
                );
                if (reasonEntity == null)
                {
                    return false;
                }

                ReasonConverter.ConvertModelToEntity(reasonDTO, ref reasonEntity);
                _dbKiloTaxiContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    $"Error occurred while updating reason with Id: {reasonDTO.Id}"
                );
                throw;
            }
        }

        public ReasonDTO GetReasonById(int id)
        {
            try
            {
                var reasonEntity = _dbKiloTaxiContext.Reasons.FirstOrDefault(r => r.Id == id);

                if (reasonEntity == null)
                {
                    LoggerHelper.Instance.LogError($"Reason with Id: {id} not found.");
                    return null;
                }

                return ReasonConverter.ConvertEntityToModel(reasonEntity);
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    $"Error occurred while fetching reason with Id: {id}"
                );
                throw;
            }
        }

        public bool DeleteReason(int id)
        {
            try
            {
                var reasonEntity = _dbKiloTaxiContext.Reasons.FirstOrDefault(r => r.Id == id);
                if (reasonEntity == null)
                {
                    return false;
                }

                _dbKiloTaxiContext.Reasons.Remove(reasonEntity);
                _dbKiloTaxiContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    $"Error occurred while deleting reason with Id: {id}"
                );
                throw;
            }
        }
    }
}
