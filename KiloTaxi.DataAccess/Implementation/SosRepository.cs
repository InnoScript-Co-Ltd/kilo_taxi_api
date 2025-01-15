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

namespace KiloTaxi.DataAccess.Implementation;

public class SosRepository : ISosRepository
{
    private readonly DbKiloTaxiContext _dbKiloTaxiContext;

    public SosRepository(DbKiloTaxiContext dbKiloTaxiContext)
    {
        _dbKiloTaxiContext = dbKiloTaxiContext;
    }

    public ResponseDTO<SosPagingDTO> GetAllSosList(PageSortParam pageSortParam)
    {
        try
        {
            var query = _dbKiloTaxiContext.Sos
                .Include(s=>s.Reason)
                .AsQueryable();
            if (!string.IsNullOrEmpty(pageSortParam.SearchTerm))
            {
                query = query.Where(p =>
                    p.Address.Contains(pageSortParam.SearchTerm)
                    || p.Status.Contains(pageSortParam.SearchTerm)
                    || p.UserType.Contains(pageSortParam.SearchTerm)
                    );
            }

            int totalCount = query.Count();
            if (!string.IsNullOrEmpty(pageSortParam.SortField))
            {
                var param = Expression.Parameter(typeof(Sos), "p");
                var property = Expression.Property(param, pageSortParam.SortField);
                var sortExpression = Expression.Lambda(property, param);

                string sortMethod =
                    pageSortParam.SortDir == SortDirection.ASC ? "OrderBy" : "OrderByDescending";
                var orderByMethod = typeof(Queryable)
                    .GetMethods()
                    .Single(m => m.Name == sortMethod && m.GetParameters().Length == 2)
                    .MakeGenericMethod(typeof(Sos), property.Type);
                query =
                    (IQueryable<Sos>)(
                        orderByMethod.Invoke(
                            _dbKiloTaxiContext,
                            new object[] { query, sortExpression }
                        ) ?? Enumerable.Empty<Sos>().AsQueryable()
                    );
            }

            if (query.Count() > pageSortParam.PageSize)
            {
                query = query
                    .Skip((pageSortParam.CurrentPage - 1) * pageSortParam.PageSize)
                    .Take(pageSortParam.PageSize);
            }

            var sos = query
                .Select(sos => SosConverter.ConvertEntityToModel(sos))
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
            
            ResponseDTO<SosPagingDTO> responseDto = new ResponseDTO<SosPagingDTO>();
            responseDto.StatusCode = (int)HttpStatusCode.OK;
            responseDto.Message = "sos retrieved successfully";
            responseDto.TimeStamp = DateTime.Now;
            responseDto.Payload = new SosPagingDTO { Paging = pagingResult, Sos = sos };
            return responseDto;
        }
        catch (Exception ex)
        {
            LoggerHelper.Instance.LogError(ex, "Error occurred while fetching all sos.");
            throw;
        }
    }
    
    public SosInfoDTO GetSosById(int id)
    {
        try
        {
            var sosEntity = _dbKiloTaxiContext.Sos.FirstOrDefault(s => s.Id == id);
            return SosConverter.ConvertEntityToModel(sosEntity);
        }
        catch (Exception ex)
        {
            LoggerHelper.Instance.LogError(ex, $"Error occurred while fetching sos with Id: {id}");
            throw;
        }
    }
    
    public SosInfoDTO CreateSos(SosFormDTO sosFormDTO)
    {
        try
        {
            var sosEntity = new Sos();
            
            SosConverter.ConvertModelToEntity(sosFormDTO, ref sosEntity);

            _dbKiloTaxiContext.Sos.Add(sosEntity);
            _dbKiloTaxiContext.SaveChanges();

            sosFormDTO.Id = sosEntity.Id;
            var sosInfoDTO = SosConverter.ConvertEntityToModel(sosEntity);
            return sosInfoDTO;
        }
        catch (Exception ex)
        {
            LoggerHelper.Instance.LogError(ex, "Error occurred while creating sos.");
            throw;
        }
    }
    public bool UpdateSos(SosFormDTO sosFormDTO)
    {
        try
        {
            var sosEntity = _dbKiloTaxiContext.Sos.FirstOrDefault(s => s.Id == sosFormDTO.Id);
            if (sosEntity == null) return false;
            SosConverter.ConvertModelToEntity(sosFormDTO, ref sosEntity);
            _dbKiloTaxiContext.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            LoggerHelper.Instance.LogError(ex, "Error occurred while updating sos.");
            throw;
        }
    }

    public bool DeleteSos(int id)
    {
        try
        {
            var sosEntity = _dbKiloTaxiContext.Sos.FirstOrDefault(s => s.Id == id);
            if (sosEntity == null) return false;

            _dbKiloTaxiContext.Sos.Remove(sosEntity);
            _dbKiloTaxiContext.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            LoggerHelper.Instance.LogError(ex, $"Error occurred while deleting sos with Id: {id}");
            throw;
        }
    }
}