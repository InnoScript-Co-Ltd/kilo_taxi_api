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
    public class ReasonRepository : IReasonRepository
    {
        private readonly DbKiloTaxiContext _dbKiloTaxiContext;

        public ReasonRepository(DbKiloTaxiContext dbContext)
        {
            _dbKiloTaxiContext = dbContext;
        }

        public ResponseDTO<ReasonPagingDTO> GetAllReason(PageSortParam pageSortParam)
        {
            try
        {
            var query = _dbKiloTaxiContext.Reasons.AsQueryable();
            if (!string.IsNullOrEmpty(pageSortParam.SearchTerm))
            {
                query = query.Where(p =>
                    p.Name.Contains(pageSortParam.SearchTerm));
            }

            int totalCount = query.Count();
            if (!string.IsNullOrEmpty(pageSortParam.SortField))
            {
                var param = Expression.Parameter(typeof(Reason), "p");
                var property = Expression.Property(param, pageSortParam.SortField);
                var sortExpression = Expression.Lambda(property, param);

                string sortMethod =
                    pageSortParam.SortDir == SortDirection.ASC ? "OrderBy" : "OrderByDescending";
                var orderByMethod = typeof(Queryable)
                    .GetMethods()
                    .Single(m => m.Name == sortMethod && m.GetParameters().Length == 2)
                    .MakeGenericMethod(typeof(Reason), property.Type);
                query =
                    (IQueryable<Reason>)(
                        orderByMethod.Invoke(
                            _dbKiloTaxiContext,
                            new object[] { query, sortExpression }
                        ) ?? Enumerable.Empty<Reason>().AsQueryable()
                    );
            }

            if (query.Count() > pageSortParam.PageSize)
            {
                query = query
                    .Skip((pageSortParam.CurrentPage - 1) * pageSortParam.PageSize)
                    .Take(pageSortParam.PageSize);
            }

            var reason = query
                .Select(reason => ReasonConverter.ConvertEntityToModel(reason))
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
            
            ResponseDTO<ReasonPagingDTO> responseDto = new ResponseDTO<ReasonPagingDTO>();
            responseDto.StatusCode = (int)HttpStatusCode.OK;
            responseDto.Message = "reasons retrieved successfully";
            responseDto.TimeStamp = DateTime.Now;
            responseDto.Payload = new ReasonPagingDTO { Paging = pagingResult, Reasons = reason };
            return responseDto;
        }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(ex, "Error occurred while fetching all reasons.");
                throw;
            }
        }

        public ReasonInfoDTO CreateReason(ReasonFormDTO reasonFormDTO)
        {
            try
            {
                Reason reasonEntity = new Reason();
                ReasonConverter.ConvertModelToEntity(reasonFormDTO, ref reasonEntity);

                _dbKiloTaxiContext.Add(reasonEntity);
                _dbKiloTaxiContext.SaveChanges();

                reasonFormDTO.Id = reasonEntity.Id;

                LoggerHelper.Instance.LogInfo(
                    $"Reason added successfully with Id: {reasonEntity.Id}"
                );

                var reasonInforDTO = ReasonConverter.ConvertEntityToModel(reasonEntity);
                return reasonInforDTO;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(ex, "Error occurred while adding Reason.");
                throw;
            }
        }

        public bool UpdateReason(ReasonFormDTO reasonFormDTO)
        {
            try
            {
                var reasonEntity = _dbKiloTaxiContext.Reasons.FirstOrDefault(r =>
                    r.Id == reasonFormDTO.Id
                );
                if (reasonEntity == null)
                {
                    return false;
                }

                ReasonConverter.ConvertModelToEntity(reasonFormDTO, ref reasonEntity);
                _dbKiloTaxiContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    $"Error occurred while updating reason with Id: {reasonFormDTO.Id}"
                );
                throw;
            }
        }

        public ReasonInfoDTO GetReasonById(int id)
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
