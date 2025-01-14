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
    public class OrderRepository : IOrderRepository
    {
        private readonly DbKiloTaxiContext _dbKiloTaxiContext;

        public OrderRepository(DbKiloTaxiContext dbKiloTaxiContext)
        {
            _dbKiloTaxiContext = dbKiloTaxiContext;
        }

        public ResponseDTO<OrderPagingDTO> GetAllOrder(PageSortParam pageSortParam)
        {
            try
            {
                var query = _dbKiloTaxiContext
                    .Orders.Include(o => o.Customer)
                    .Include(o => o.Driver)
                    .AsQueryable();
                if (!string.IsNullOrEmpty(pageSortParam.SearchTerm))
                {
                    query = query.Where(p => p.Status.Contains(pageSortParam.SearchTerm));
                }

                int totalCount = query.Count();
                if (!string.IsNullOrEmpty(pageSortParam.SortField))
                {
                    var param = Expression.Parameter(typeof(Order), "p");
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
                        .MakeGenericMethod(typeof(Order), property.Type);

                    query =
                        (IQueryable<Order>)
                            orderByMethod.Invoke(null, new object[] { query, sortExpression });
                }

                if (query.Count() > pageSortParam.PageSize)
                {
                    query = query
                        .Skip((pageSortParam.CurrentPage - 1) * pageSortParam.PageSize)
                        .Take(pageSortParam.PageSize);
                }

                var orders = query
                    .Select(order => OrderConverter.ConvertEntityToModel(order))
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
                // return new OrderPagingDTO() { Paging = pagingResult, Orders = orders };
                ResponseDTO<OrderPagingDTO> responseDto = new ResponseDTO<OrderPagingDTO>();
                responseDto.StatusCode = (int)HttpStatusCode.OK;
                responseDto.Message = "Orders retrieved successfully";
                responseDto.TimeStamp = DateTime.Now;
                responseDto.Payload = new OrderPagingDTO { Paging = pagingResult, Orders = orders };
                return responseDto;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(ex, "Error occurred while fetching all orders.");
                throw;
            }
        }

        public OrderInfoDTO AddOrder(OrderFormDTO orderFormDTO)
        {
            try
            {
                Order orderEntity = new Order();
                DateTime createDate = DateTime.Now;
                orderFormDTO.CreatedDate = createDate;
                OrderConverter.ConvertModelToEntity(orderFormDTO, ref orderEntity);

                _dbKiloTaxiContext.Add(orderEntity);
                _dbKiloTaxiContext.SaveChanges();

                orderFormDTO.Id = orderEntity.Id;

                LoggerHelper.Instance.LogInfo(
                    $"Order added successfully with Id: {orderEntity.Id}"
                );
                var orderInfoDTO = OrderConverter.ConvertEntityToModel(orderEntity);

                return orderInfoDTO;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(ex, "Error occurred while adding admin.");
                throw;
            }
        }

        public bool UpdateOrder(OrderFormDTO orderFormDTO)
        {
            try
            {
                var orderEntity = _dbKiloTaxiContext.Orders.FirstOrDefault(order =>
                    order.Id == orderFormDTO.Id
                );
                if (orderEntity == null)
                {
                    return false;
                }

                OrderConverter.ConvertModelToEntity(orderFormDTO, ref orderEntity);
                _dbKiloTaxiContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    $"Error occurred while updating order with Id: {orderFormDTO.Id}"
                );
                throw;
            }
        }

        public OrderInfoDTO GetOrderById(int id)
        {
            try
            {
                var orderDTO = OrderConverter.ConvertEntityToModel(
                    _dbKiloTaxiContext.Orders.FirstOrDefault(order => order.Id == id)
                );

                if (orderDTO == null)
                {
                    LoggerHelper.Instance.LogError($"Order with Id: {id} not found.");
                    return null;
                }

                return orderDTO;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    $"Error occurred while fetching order with Id: {id}"
                );
                throw;
            }
        }

        public bool DeleteOrder(int id)
        {
            try
            {
                var orderEntity = _dbKiloTaxiContext.Orders.FirstOrDefault(order => order.Id == id);
                if (orderEntity == null)
                {
                    return false;
                }

                _dbKiloTaxiContext.Orders.Remove(orderEntity);
                _dbKiloTaxiContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    $"Error occurred while deleting order with Id: {id}"
                );
                throw;
            }
        }
    }
}
