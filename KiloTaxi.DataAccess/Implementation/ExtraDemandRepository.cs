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
    public class ExtraDemandRepository : IExtraDemandRepository
    {
        private readonly DbKiloTaxiContext _dbKiloTaxiContext;

        public ExtraDemandRepository(DbKiloTaxiContext dbContext)
        {
            _dbKiloTaxiContext = dbContext;
        }

        public ResponseDTO<ExtraDemandPagingDTO> GetAllExtraDemand(PageSortParam pageSortParam)
        {
            try
            {
                var query = _dbKiloTaxiContext.ExtraDemands.AsQueryable();

                if (!string.IsNullOrEmpty(pageSortParam.SearchTerm))
                {
                    query = query.Where(extraDemand =>
                        extraDemand.Title.Contains(pageSortParam.SearchTerm)
                        || extraDemand.Description.Contains(pageSortParam.SearchTerm)
                    );
                }

                int totalCount = query.Count();

                if (!string.IsNullOrEmpty(pageSortParam.SortField))
                {
                    var param = Expression.Parameter(typeof(ExtraDemand), "extraDemand");
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
                        .MakeGenericMethod(typeof(ExtraDemand), property.Type);

                    query =
                        (IQueryable<ExtraDemand>)
                            orderByMethod.Invoke(null, new object[] { query, sortExpression });
                }

                if (query.Count() > pageSortParam.PageSize)
                {
                    query = query
                        .Skip((pageSortParam.CurrentPage - 1) * pageSortParam.PageSize)
                        .Take(pageSortParam.PageSize);
                }

                var extraDemands = query.Select(ExtraDemandConverter.ConvertEntityToModel).ToList();

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

                ResponseDTO<ExtraDemandPagingDTO> responseDto = new ResponseDTO<ExtraDemandPagingDTO>();
                responseDto.StatusCode = (int)HttpStatusCode.OK;
                responseDto.Message = "extra demands retrieved successfully";
                responseDto.TimeStamp = DateTime.Now;
                responseDto.Payload = new ExtraDemandPagingDTO { Paging = pagingResult, ExtraDemands = extraDemands };
                return responseDto;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    "Error occurred while fetching all extraDemands."
                );
                throw;
            }
        }

        public ExtraDemandInfoDTO CreateExtraDemand(ExtraDemandFormDTO extraDemandFormDTO)
        {
            try
            {
                ExtraDemand extraDemandEntity = new ExtraDemand();
                ExtraDemandConverter.ConvertModelToEntity(extraDemandFormDTO, ref extraDemandEntity);
                
                _dbKiloTaxiContext.Add(extraDemandEntity);
                _dbKiloTaxiContext.SaveChanges();

                extraDemandFormDTO.Id = extraDemandEntity.Id;

                LoggerHelper.Instance.LogInfo(
                    $"ExtraDemand added successfully with Id: {extraDemandEntity.Id}"
                );

                var extraDemandInfoDTO = ExtraDemandConverter.ConvertEntityToModel(extraDemandEntity);
                return extraDemandInfoDTO;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(ex, "Error occurred while adding extraDemand.");
                throw;
            }
        }

        public bool UpdateExtraDemand(ExtraDemandFormDTO extraDemandFormDTO)
        {
            try
            {
                var extraDemandEntity = _dbKiloTaxiContext.ExtraDemands.FirstOrDefault(extraDemand =>
                    extraDemand.Id == extraDemandFormDTO.Id
                );
                if (extraDemandEntity == null)
                {
                    return false;
                }

                ExtraDemandConverter.ConvertModelToEntity(extraDemandFormDTO, ref extraDemandEntity);
                _dbKiloTaxiContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    $"Error occurred while updating extraDemand with Id: {extraDemandFormDTO.Id}"
                );
                throw;
            }
        }

        public ExtraDemandInfoDTO GetExtraDemandById(int id)
        {
            try
            {
                var extraDemandEntity = _dbKiloTaxiContext
                    .ExtraDemands.FirstOrDefault(extraDemand => extraDemand.Id == id);

                if (extraDemandEntity == null)
                {
                    LoggerHelper.Instance.LogError($"ExtraDemand with Id: {id} not found.");
                    return null;
                }

                return ExtraDemandConverter.ConvertEntityToModel(extraDemandEntity);
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    $"Error occurred while fetching extraDemand with Id: {id}"
                );
                throw;
            }
        }

        public bool DeleteExtraDemand(int id)
        {
            try
            {
                var extraDemandEntity = _dbKiloTaxiContext.ExtraDemands.FirstOrDefault(extraDemand =>
                    extraDemand.Id == id
                );
                if (extraDemandEntity == null)
                {
                    return false;
                }

                _dbKiloTaxiContext.ExtraDemands.Remove(extraDemandEntity);
                _dbKiloTaxiContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    $"Error occurred while deleting extraDemand with Id: {id}"
                );
                throw;
            }
        }
    }
}
