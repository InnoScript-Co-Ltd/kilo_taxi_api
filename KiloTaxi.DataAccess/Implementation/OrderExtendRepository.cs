using System.Linq.Expressions;
using System.Net;
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
    public class OrderExtendRepository : IOrderExtendRepository
    {
        private readonly DbKiloTaxiContext _dbKiloTaxiContext;

        public OrderExtendRepository(DbKiloTaxiContext dbContext)
        {
            _dbKiloTaxiContext = dbContext;
        }

        public ResponseDTO<OrderExtendPagingDTO> GetAllOrderExtend(PageSortParam pageSortParam)
        {
            try
            {
                var query = _dbKiloTaxiContext.OrderExtends.Include(r => r.Order).AsQueryable();

                if (!string.IsNullOrEmpty(pageSortParam.SearchTerm))
                {
                    query = query.Where(orderExtend =>
                        orderExtend.DestinationLat.Contains(pageSortParam.SearchTerm)
                        || orderExtend.DestinationLong.Contains(pageSortParam.SearchTerm)
                        || orderExtend.DestinationLocation.Contains(pageSortParam.SearchTerm)
                    );
                }

                int totalCount = query.Count();

                if (!string.IsNullOrEmpty(pageSortParam.SortField))
                {
                    var param = Expression.Parameter(typeof(OrderExtend), "orderExtend");
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
                        .MakeGenericMethod(typeof(OrderExtend), property.Type);

                    query =
                        (IQueryable<OrderExtend>)
                            orderByMethod.Invoke(null, new object[] { query, sortExpression });
                }

                if (query.Count() > pageSortParam.PageSize)
                {
                    query = query
                        .Skip((pageSortParam.CurrentPage - 1) * pageSortParam.PageSize)
                        .Take(pageSortParam.PageSize);
                }

                var orderExtends = query.Select(OrderExtendConverter.ConvertEntityToModel).ToList();

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

                ResponseDTO<OrderExtendPagingDTO> responseDto = new ResponseDTO<OrderExtendPagingDTO>();
                responseDto.StatusCode = (int)HttpStatusCode.OK;
                responseDto.Message = "order extends retrieved successfully";
                responseDto.TimeStamp = DateTime.Now;
                responseDto.Payload = new OrderExtendPagingDTO { Paging = pagingResult, OrderExtends = orderExtends };
                return responseDto;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    "Error occurred while fetching all orderExtends."
                );
                throw;
            }
        }

        public OrderExtendInfoDTO CreateOrderExtend(OrderExtendFormDTO orderExtendFormDTO)
        {
            try
            {
                OrderExtend orderExtendEntity = new OrderExtend();
                OrderExtendConverter.ConvertModelToEntity(orderExtendFormDTO, ref orderExtendEntity);

                var order = _dbKiloTaxiContext.Orders.FirstOrDefault(s =>
                    s.Id == orderExtendFormDTO.OrderId
                );

                _dbKiloTaxiContext.Add(orderExtendEntity);
                _dbKiloTaxiContext.SaveChanges();

                orderExtendFormDTO.Id = orderExtendEntity.Id;

                LoggerHelper.Instance.LogInfo(
                    $"OrderExtend added successfully with Id: {orderExtendEntity.Id}"
                );

                var orderExtendInfoDTO = OrderExtendConverter.ConvertEntityToModel(orderExtendEntity);
                return orderExtendInfoDTO;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(ex, "Error occurred while adding orderExtend.");
                throw;
            }
        }

        public bool UpdateOrderExtend(OrderExtendFormDTO orderExtendFormDTO)
        {
            try
            {
                var orderExtendEntity = _dbKiloTaxiContext.OrderExtends.FirstOrDefault(orderExtend =>
                    orderExtend.Id == orderExtendFormDTO.Id
                );
                if (orderExtendEntity == null)
                {
                    return false;
                }

                OrderExtendConverter.ConvertModelToEntity(orderExtendFormDTO, ref orderExtendEntity);
                _dbKiloTaxiContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    $"Error occurred while updating orderExtend with Id: {orderExtendFormDTO.Id}"
                );
                throw;
            }
        }

        public OrderExtendInfoDTO GetOrderExtendById(int id)
        {
            try
            {
                var orderExtendEntity = _dbKiloTaxiContext
                    .OrderExtends.Include(r => r.Order)
                    .FirstOrDefault(orderExtend => orderExtend.Id == id);

                if (orderExtendEntity == null)
                {
                    LoggerHelper.Instance.LogError($"OrderExtend with Id: {id} not found.");
                    return null;
                }

                return OrderExtendConverter.ConvertEntityToModel(orderExtendEntity);
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    $"Error occurred while fetching orderExtend with Id: {id}"
                );
                throw;
            }
        }

        public bool DeleteOrderExtend(int id)
        {
            try
            {
                var orderExtendEntity = _dbKiloTaxiContext.OrderExtends.FirstOrDefault(orderExtend =>
                    orderExtend.Id == id
                );
                if (orderExtendEntity == null)
                {
                    return false;
                }

                _dbKiloTaxiContext.OrderExtends.Remove(orderExtendEntity);
                _dbKiloTaxiContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    $"Error occurred while deleting orderExtend with Id: {id}"
                );
                throw;
            }
        }
    }
}
