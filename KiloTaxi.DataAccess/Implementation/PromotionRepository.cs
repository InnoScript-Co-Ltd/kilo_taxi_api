using System.Linq.Expressions;
using System.Net;
using KiloTaxi.Common.Enums;
using KiloTaxi.Converter;
using KiloTaxi.DataAccess.Interface;
using KiloTaxi.EntityFramework;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;
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

        public ResponseDTO<PromotionPagingDTO> GetAllPromotion(PageSortParam pageSortParam)
        {
            try
            {
                var query = _dbKiloTaxiContext
                    .Promotions.Include(p => p.PromotionUsers)
                    .ThenInclude(pu => pu.Customer) // Include related Customer data
                    .AsQueryable();

                if (!string.IsNullOrEmpty(pageSortParam.SearchTerm))
                {
                    query = query.Where(promotion =>
                        promotion.PromoCode.Contains(pageSortParam.SearchTerm)
                    );
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

                var promotions = query
                    .ToList() // Fetch data before converting
                    .Select(PromotionConverter.ConvertEntityToModel)
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

                ResponseDTO<PromotionPagingDTO> responseDto = new ResponseDTO<PromotionPagingDTO>();
                responseDto.StatusCode = (int)HttpStatusCode.OK;
                responseDto.Message = "promotions retrieved successfully";
                responseDto.TimeStamp = DateTime.Now;
                responseDto.Payload = new PromotionPagingDTO { Paging = pagingResult, Promotions = promotions };
                return responseDto;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(ex, "Error occurred while fetching all promotions.");
                throw;
            }
        }

        public PromotionInfoDTO AddPromotion(PromotionFormDTO promotionFormDTO)
        {
            try
            {
                promotionFormDTO.CustomerIds = promotionFormDTO.CustomerIds?.Distinct().ToList();

                Promotion promotionEntity = new Promotion();
                PromotionConverter.ConvertModelToEntity(promotionFormDTO, ref promotionEntity);

                promotionEntity.PromotionUsers.Clear();

                // Validate and add PromotionUsers
                if (promotionFormDTO.CustomerIds != null && promotionFormDTO.CustomerIds.Any())
                {
                    var customers = _dbKiloTaxiContext
                        .Customers.Where(c => promotionFormDTO.CustomerIds.Contains(c.Id))
                        .ToList();

                    if (customers.Count != promotionFormDTO.CustomerIds.Count)
                    {
                        throw new ArgumentException("One or more customer IDs are invalid.");
                    }

                    foreach (var customer in customers)
                    {
                        promotionEntity.PromotionUsers.Add(
                            new PromotionUser
                            {
                                CustomerId = customer.Id,
                                PromotionId = promotionEntity.Id,
                            }
                        );
                    }
                }

                _dbKiloTaxiContext.Add(promotionEntity);
                _dbKiloTaxiContext.SaveChanges();

                promotionFormDTO.CustomerNames = _dbKiloTaxiContext
                    .Customers.Where(c => promotionFormDTO.CustomerIds.Contains(c.Id))
                    .Select(c => c.Name)
                    .ToList();

                promotionFormDTO.Id = promotionEntity.Id;

                LoggerHelper.Instance.LogInfo(
                    $"Promotion added successfully with Id: {promotionEntity.Id}"
                );

                var promotionInfoDTO = PromotionConverter.ConvertEntityToModel(promotionEntity);
                return promotionInfoDTO;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(ex, "Error occurred while adding promotion.");
                throw;
            }
        }

        public bool UpdatePromotion(PromotionFormDTO promotionFormDTO)
        {
            try
            {
                var promotionEntity = _dbKiloTaxiContext
                    .Promotions.Include(p => p.PromotionUsers)
                    .FirstOrDefault(promotion => promotion.Id == promotionFormDTO.Id);

                if (promotionEntity == null)
                {
                    return false;
                }

                PromotionConverter.ConvertModelToEntity(promotionFormDTO, ref promotionEntity);

                if (promotionFormDTO.CustomerIds != null)
                {
                    var validCustomers = _dbKiloTaxiContext
                        .Customers.Where(c => promotionFormDTO.CustomerIds.Contains(c.Id))
                        .ToList();

                    if (validCustomers.Count != promotionFormDTO.CustomerIds.Count)
                    {
                        throw new ArgumentException("One or more customer IDs are invalid.");
                    }

                    promotionEntity.PromotionUsers.Clear();

                    promotionEntity.PromotionUsers = validCustomers
                        .Select(customer => new PromotionUser
                        {
                            CustomerId = customer.Id,
                            PromotionId = promotionEntity.Id,
                        })
                        .ToList();
                }
                else
                {
                    promotionEntity.PromotionUsers.Clear();
                }

                _dbKiloTaxiContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    $"Error occurred while updating promotion with Id: {promotionFormDTO.Id}"
                );
                throw;
            }
        }

        public PromotionInfoDTO GetPromotionById(int id)
        {
            try
            {
                var promotionEntity = _dbKiloTaxiContext
                    .Promotions.Include(p => p.PromotionUsers)
                    .ThenInclude(pu => pu.Customer)
                    .FirstOrDefault(promotion => promotion.Id == id);

                if (promotionEntity == null)
                {
                    LoggerHelper.Instance.LogError($"Promotion with Id: {id} not found.");
                    return null;
                }

                var promotionDTO = PromotionConverter.ConvertEntityToModel(promotionEntity);

                promotionDTO.CustomerNames = promotionEntity
                    .PromotionUsers.Select(pu => pu.Customer.Name)
                    .ToList();

                return promotionDTO;
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
                var promotionEntity = _dbKiloTaxiContext
                    .Promotions.Include(p => p.PromotionUsers)
                    .FirstOrDefault(promotion => promotion.Id == id);
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
