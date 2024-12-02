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
    public class PromotionUsageRepository : IPromotionUsageRepository
    {
        private readonly DbKiloTaxiContext _dbKiloTaxiContext;

        public PromotionUsageRepository(DbKiloTaxiContext dbContext)
        {
            _dbKiloTaxiContext = dbContext;
        }

        public PromotionUsagePagingDTO GetAllPromotionUsage(PageSortParam pageSortParam)
        {
            try
            {
                var query = _dbKiloTaxiContext.PromotionUsages.AsQueryable();

                int totalCount = query.Count();

                if (!string.IsNullOrEmpty(pageSortParam.SortField))
                {
                    var param = Expression.Parameter(typeof(PromotionUsage), "promotionUsage");
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
                        .MakeGenericMethod(typeof(PromotionUsage), property.Type);

                    query =
                        (IQueryable<PromotionUsage>)
                            orderByMethod.Invoke(null, new object[] { query, sortExpression });
                }

                if (query.Count() > pageSortParam.PageSize)
                {
                    query = query
                        .Skip((pageSortParam.CurrentPage - 1) * pageSortParam.PageSize)
                        .Take(pageSortParam.PageSize);
                }

                var promotionUsages = query
                    .Select(PromotionUsageConverter.ConvertEntityToModel)
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

                return new PromotionUsagePagingDTO
                {
                    Paging = pagingResult,
                    promotionUsages = promotionUsages,
                };
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(ex, "Error occurred while fetching all promotions.");
                throw;
            }
        }

        public PromotionUsageDTO AddPromotionUsage(PromotionUsageDTO promotionUsageDTO)
        {
            try
            {
                PromotionUsage promotionUsageEntity = new PromotionUsage();
                PromotionUsageConverter.ConvertModelToEntity(
                    promotionUsageDTO,
                    ref promotionUsageEntity
                );

                var customer = _dbKiloTaxiContext.Customers.FirstOrDefault(c =>
                    c.Id == promotionUsageDTO.CustomerId
                );

                _dbKiloTaxiContext.Add(promotionUsageEntity);
                _dbKiloTaxiContext.SaveChanges();

                promotionUsageDTO.Id = promotionUsageEntity.Id;

                LoggerHelper.Instance.LogInfo(
                    $"Promotion added successfully with Id: {promotionUsageEntity.Id}"
                );

                return promotionUsageDTO;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(ex, "Error occurred while adding promotion Usage.");
                throw;
            }
        }

        public bool UpdatePromotionUsage(PromotionUsageDTO promotionUsageDTO)
        {
            try
            {
                var promotionUsageEntity = _dbKiloTaxiContext.PromotionUsages.FirstOrDefault(
                    usage => usage.Id == promotionUsageDTO.Id
                );
                if (promotionUsageDTO == null)
                {
                    return false;
                }

                PromotionUsageConverter.ConvertModelToEntity(
                    promotionUsageDTO,
                    ref promotionUsageEntity
                );
                _dbKiloTaxiContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    $"Error occurred while updating promotion usage with Id: {promotionUsageDTO.Id}"
                );
                throw;
            }
        }

        public PromotionUsageDTO GetPromotionUsageById(int id)
        {
            try
            {
                var promotionUsageDTO = PromotionUsageConverter.ConvertEntityToModel(
                    _dbKiloTaxiContext.PromotionUsages.FirstOrDefault(usage => usage.Id == id)
                );

                if (promotionUsageDTO == null)
                {
                    LoggerHelper.Instance.LogError($"Promotion Usage with Id: {id} not found.");
                    return null;
                }

                return promotionUsageDTO;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    $"Error occurred while fetching promotion with Id: {id}"
                );
                throw;
            }
        }

        public bool DeletePromotionUsage(int id)
        {
            try
            {
                var promotionUsageEntity = _dbKiloTaxiContext.PromotionUsages.FirstOrDefault(
                    usage => usage.Id == id
                );
                if (promotionUsageEntity == null)
                {
                    return false;
                }

                _dbKiloTaxiContext.PromotionUsages.Remove(promotionUsageEntity);
                _dbKiloTaxiContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    $"Error occurred while deleting promotion with Id: {id}"
                );
                throw;
            }
        }
    }
}
