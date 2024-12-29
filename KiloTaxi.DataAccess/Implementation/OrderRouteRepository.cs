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
    public class OrderRouteRepository : IOrderRouteRepository
    {
        private readonly DbKiloTaxiContext _dbKiloTaxiContext;

        public OrderRouteRepository(DbKiloTaxiContext dbContext)
        {
            _dbKiloTaxiContext = dbContext;
        }

        public OrderRoutePagingDTO GetAllOrderRoute(PageSortParam pageSortParam)
        {
            try
            {
                var query = _dbKiloTaxiContext.OrderRoutes.Include(r => r.Order).AsQueryable();

                if (!string.IsNullOrEmpty(pageSortParam.SearchTerm))
                {
                    query = query.Where(orderRoute =>
                        orderRoute.Lat.Contains(pageSortParam.SearchTerm)
                        || orderRoute.Long.Contains(pageSortParam.SearchTerm)
                    );
                }

                int totalCount = query.Count();

                if (!string.IsNullOrEmpty(pageSortParam.SortField))
                {
                    var param = Expression.Parameter(typeof(OrderRoute), "orderRoute");
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
                        .MakeGenericMethod(typeof(OrderRoute), property.Type);

                    query =
                        (IQueryable<OrderRoute>)
                            orderByMethod.Invoke(null, new object[] { query, sortExpression });
                }

                if (query.Count() > pageSortParam.PageSize)
                {
                    query = query
                        .Skip((pageSortParam.CurrentPage - 1) * pageSortParam.PageSize)
                        .Take(pageSortParam.PageSize);
                }

                var orderRoutes = query.Select(OrderRouteConverter.ConvertEntityToModel).ToList();

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

                return new OrderRoutePagingDTO { Paging = pagingResult, OrderRoutes = orderRoutes };
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    "Error occurred while fetching all orderRoutes."
                );
                throw;
            }
        }

        public OrderRouteDTO CreateOrderRoute(OrderRouteDTO orderRouteDTO)
        {
            try
            {
                OrderRoute orderRouteEntity = new OrderRoute();
                OrderRouteConverter.ConvertModelToEntity(orderRouteDTO, ref orderRouteEntity);

                var order = _dbKiloTaxiContext.Orders.FirstOrDefault(s =>
                    s.Id == orderRouteDTO.OrderId
                );

                _dbKiloTaxiContext.Add(orderRouteEntity);
                _dbKiloTaxiContext.SaveChanges();

                orderRouteDTO.Id = orderRouteEntity.Id;

                LoggerHelper.Instance.LogInfo(
                    $"OrderRoute added successfully with Id: {orderRouteEntity.Id}"
                );

                return orderRouteDTO;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(ex, "Error occurred while adding orderRoute.");
                throw;
            }
        }

        public bool UpdateOrderRoute(OrderRouteDTO orderRouteDTO)
        {
            try
            {
                var orderRouteEntity = _dbKiloTaxiContext.OrderRoutes.FirstOrDefault(orderRoute =>
                    orderRoute.Id == orderRouteDTO.Id
                );
                if (orderRouteEntity == null)
                {
                    return false;
                }

                OrderRouteConverter.ConvertModelToEntity(orderRouteDTO, ref orderRouteEntity);
                _dbKiloTaxiContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    $"Error occurred while updating orderRoute with Id: {orderRouteDTO.Id}"
                );
                throw;
            }
        }

        public OrderRouteDTO GetOrderRouteById(int id)
        {
            try
            {
                var orderRouteEntity = _dbKiloTaxiContext
                    .OrderRoutes.Include(r => r.Order)
                    .FirstOrDefault(orderRoute => orderRoute.Id == id);

                if (orderRouteEntity == null)
                {
                    LoggerHelper.Instance.LogError($"OrderRoute with Id: {id} not found.");
                    return null;
                }

                return OrderRouteConverter.ConvertEntityToModel(orderRouteEntity);
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    $"Error occurred while fetching orderRoute with Id: {id}"
                );
                throw;
            }
        }

        public bool DeleteOrderRoute(int id)
        {
            try
            {
                var orderRouteEntity = _dbKiloTaxiContext.OrderRoutes.FirstOrDefault(orderRoute =>
                    orderRoute.Id == id
                );
                if (orderRouteEntity == null)
                {
                    return false;
                }

                _dbKiloTaxiContext.OrderRoutes.Remove(orderRouteEntity);
                _dbKiloTaxiContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    $"Error occurred while deleting orderRoute with Id: {id}"
                );
                throw;
            }
        }
    }
}
