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
    public class PromotionRepository : IPromotionRepository
    {
        private readonly DbKiloTaxiContext _dbKiloTaxiContext;

        public PromotionRepository(DbKiloTaxiContext dbContext)
        {
            _dbKiloTaxiContext = dbContext;
        }

        public PromotionPagingDTO GetAllPromotion(PageSortParam pageSortParam)
        {
            try
            {
                var query = _dbKiloTaxiContext.Promotions.AsQueryable();

                if (!string.IsNullOrEmpty(pageSortParam.SearchTerm))
                {
                    query = query.Where(promotion =>
                        promotion.PromoCode.Contains(pageSortParam.SearchTerm));
                }

                int totalCount = query.Count();

                if (!string.IsNullOrEmpty(pageSortParam.SortField))
                {
                    var param = Expression.Parameter(typeof(Promotion), "promotion");
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
                        .MakeGenericMethod(typeof(Promotion), property.Type);

                    query =
                        (IQueryable<Promotion>)
                            orderByMethod.Invoke(null, new object[] { query, sortExpression });
                }

                if (query.Count() > pageSortParam.PageSize)
                {
                    query = query
                        .Skip((pageSortParam.CurrentPage - 1) * pageSortParam.PageSize)
                        .Take(pageSortParam.PageSize);
                }

                var promotions = query.Select(PromotionConverter.ConvertEntityToModel).ToList();

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

                return new PromotionPagingDTO { Paging = pagingResult, Promotions = promotions };
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(ex, "Error occurred while fetching all promotions.");
                throw;
            }
        }

        public PromotionDTO AddPromotion(PromotionDTO promotionDTO)
        {
            try
            {
                Promotion promotionEntity = new Promotion();
                PromotionConverter.ConvertModelToEntity(promotionDTO, ref promotionEntity);

              
                _dbKiloTaxiContext.Add(promotionEntity);
                _dbKiloTaxiContext.SaveChanges();

                promotionDTO.Id = promotionEntity.Id;

                LoggerHelper.Instance.LogInfo(
                    $"Promotion added successfully with Id: {promotionEntity.Id}"
                );

                return promotionDTO;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(ex, "Error occurred while adding promotion.");
                throw;
            }
        }

        public bool UpdatePromotion(PromotionDTO promotionDTO)
        {
            try
            {
                var promotionEntity = _dbKiloTaxiContext.Promotions.FirstOrDefault(promotion =>
                    promotion.Id == promotionDTO.Id
                );
                if (promotionEntity == null)
                {
                    return false;
                }

                PromotionConverter.ConvertModelToEntity(promotionDTO, ref promotionEntity);
                _dbKiloTaxiContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    $"Error occurred while updating promotion with Id: {promotionDTO.Id}"
                );
                throw;
            }
        }

        public PromotionDTO GetPromotionById(int id)
        {
            try
            {
                var promotionEntity = _dbKiloTaxiContext
                    .Promotions
                    .FirstOrDefault(promotion => promotion.Id == id);
                if (promotionEntity == null)
                {
                    LoggerHelper.Instance.LogError($"Promotion with Id: {id} not found.");
                    return null;
                }

                return PromotionConverter.ConvertEntityToModel(promotionEntity);
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

        public bool DeletePromotion(int id)
        {
            try
            {
                var promotionEntity = _dbKiloTaxiContext.Promotions.FirstOrDefault(promotion =>
                    promotion.Id == id
                );
                if (promotionEntity == null)
                {
                    return false;
                }

                _dbKiloTaxiContext.Promotions.Remove(promotionEntity);
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
