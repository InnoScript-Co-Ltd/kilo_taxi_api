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
    public class OrderExtraDemandRepository : IOrderExtraDemandRepository
    {
        private readonly DbKiloTaxiContext _dbKiloTaxiContext;

        public OrderExtraDemandRepository(DbKiloTaxiContext dbContext)
        {
            _dbKiloTaxiContext = dbContext;
        }

        public ResponseDTO<OrderExtraDemandPagingDTO> GetAllOrderExtraDemand(PageSortParam pageSortParam)
        {
            try
            {
                var query = _dbKiloTaxiContext.OrderExtraDemands.AsQueryable();

                int totalCount = query.Count();

                if (!string.IsNullOrEmpty(pageSortParam.SortField))
                {
                    var param = Expression.Parameter(typeof(OrderExtraDemand), "orderExtraDemand");
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
                        .MakeGenericMethod(typeof(OrderExtraDemand), property.Type);

                    query =
                        (IQueryable<OrderExtraDemand>)
                            orderByMethod.Invoke(null, new object[] { query, sortExpression });
                }

                if (query.Count() > pageSortParam.PageSize)
                {
                    query = query
                        .Skip((pageSortParam.CurrentPage - 1) * pageSortParam.PageSize)
                        .Take(pageSortParam.PageSize);
                }

                var orderExtraDemands = query.Select(OrderExtraDemandConverter.ConvertEntityToModel).ToList();

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

                ResponseDTO<OrderExtraDemandPagingDTO> responseDto = new ResponseDTO<OrderExtraDemandPagingDTO>();
                responseDto.StatusCode = (int)HttpStatusCode.OK;
                responseDto.Message = "order extra demands retrieved successfully";
                responseDto.TimeStamp = DateTime.Now;
                responseDto.Payload = new OrderExtraDemandPagingDTO { Paging = pagingResult, OrderExtraDemands = orderExtraDemands };
                return responseDto;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    "Error occurred while fetching all orderExtraDemands."
                );
                throw;
            }
        }

        public List<OrderExtraDemandDTO> CreateOrderExtraDemand(List<OrderExtraDemandDTO> orderExtraDemandDTOList)
        {
            try
            {
                OrderExtraDemand orderExtraDemandEntity = new OrderExtraDemand();
                List<OrderExtraDemandDTO> orderExtraDemandDTOs = new List<OrderExtraDemandDTO>();
                foreach (var orderExtraDemandDTO in orderExtraDemandDTOList){
                OrderExtraDemandConverter.ConvertModelToEntity(orderExtraDemandDTO, ref orderExtraDemandEntity);
                _dbKiloTaxiContext.Add(orderExtraDemandEntity);
                _dbKiloTaxiContext.SaveChanges();
                var extraDemand = _dbKiloTaxiContext.ExtraDemands.FirstOrDefault(e => e.Id == orderExtraDemandEntity.ExtraDemandId);
                orderExtraDemandDTO.ExtraDemandDto =ExtraDemandConverter.ConvertEntityToModel(extraDemand);
                orderExtraDemandDTO.Id = orderExtraDemandEntity.Id;
                orderExtraDemandDTOs.Add(orderExtraDemandDTO);
                }
                LoggerHelper.Instance.LogInfo(
                    $"OrderExtraDemand added successfully with Id: {orderExtraDemandEntity.Id}"
                );

                return orderExtraDemandDTOs;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(ex, "Error occurred while adding orderExtraDemand.");
                throw;
            }
        }

        public bool UpdateOrderExtraDemand(OrderExtraDemandDTO orderExtraDemandDTO)
        {
            try
            {
                var orderExtraDemandEntity = _dbKiloTaxiContext.OrderExtraDemands.FirstOrDefault(orderExtraDemand =>
                    orderExtraDemand.Id == orderExtraDemandDTO.Id
                );
                if (orderExtraDemandEntity == null)
                {
                    return false;
                }

                OrderExtraDemandConverter.ConvertModelToEntity(orderExtraDemandDTO, ref orderExtraDemandEntity);
                _dbKiloTaxiContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    $"Error occurred while updating orderExtraDemand with Id: {orderExtraDemandDTO.Id}"
                );
                throw;
            }
        }

        public OrderExtraDemandInfoDTO GetOrderExtraDemandById(int id)
        {
            try
            {
                var orderExtraDemandEntity = _dbKiloTaxiContext
                    .OrderExtraDemands.Include(r => r.Order)
                    .FirstOrDefault(orderExtraDemand => orderExtraDemand.Id == id);

                if (orderExtraDemandEntity == null)
                {
                    LoggerHelper.Instance.LogError($"OrderExtraDemand with Id: {id} not found.");
                    return null;
                }

                return OrderExtraDemandConverter.ConvertEntityToModel(orderExtraDemandEntity);
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    $"Error occurred while fetching orderExtraDemand with Id: {id}"
                );
                throw;
            }
        }

        public bool DeleteOrderExtraDemand(int id)
        {
            try
            {
                var orderExtraDemandEntity = _dbKiloTaxiContext.OrderExtraDemands.FirstOrDefault(orderExtraDemand =>
                    orderExtraDemand.Id == id
                );
                if (orderExtraDemandEntity == null)
                {
                    return false;
                }

                _dbKiloTaxiContext.OrderExtraDemands.Remove(orderExtraDemandEntity);
                _dbKiloTaxiContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    $"Error occurred while deleting orderExtraDemand with Id: {id}"
                );
                throw;
            }
        }
    }
}
