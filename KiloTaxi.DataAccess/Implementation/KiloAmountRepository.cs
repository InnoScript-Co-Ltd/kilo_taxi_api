using System.Linq.Expressions;
using KiloTaxi.Converter;
using KiloTaxi.DataAccess.Interface;
using KiloTaxi.EntityFramework;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using Microsoft.EntityFrameworkCore;

namespace KiloTaxi.DataAccess.Implementation;

public class KiloAmountRepository : IKiloAmountRepository
{
    private readonly DbKiloTaxiContext _dbKiloTaxiContext;

    public KiloAmountRepository(DbKiloTaxiContext dbKiloTaxiContext)
    {
        _dbKiloTaxiContext = dbKiloTaxiContext;
    }

    public KiloAmountPagingDTO GetAllKiloAmountList(PageSortParam pageSortParam)
    {
        try
        {
            var query = _dbKiloTaxiContext.KiloAmounts.AsQueryable();

            int totalCount = query.Count();
            if (!string.IsNullOrEmpty(pageSortParam.SortField))
            {
                var param = Expression.Parameter(typeof(KiloAmount), "p");
                var property = Expression.Property(param, pageSortParam.SortField);
                var sortExpression = Expression.Lambda(property, param);

                string sortMethod =
                    pageSortParam.SortDir == SortDirection.ASC ? "OrderBy" : "OrderByDescending";
                var orderByMethod = typeof(Queryable)
                    .GetMethods()
                    .Single(m => m.Name == sortMethod && m.GetParameters().Length == 2)
                    .MakeGenericMethod(typeof(KiloAmount), property.Type);
                query =
                    (IQueryable<KiloAmount>)(
                        orderByMethod.Invoke(
                            _dbKiloTaxiContext,
                            new object[] { query, sortExpression }
                        ) ?? Enumerable.Empty<KiloAmount>().AsQueryable()
                    );
            }

            if (query.Count() > pageSortParam.PageSize)
            {
                query = query
                    .Skip((pageSortParam.CurrentPage - 1) * pageSortParam.PageSize)
                    .Take(pageSortParam.PageSize);
            }

            var kiloAmount = query
                .Select(kiloAmount => KiloAmountConverter.ConvertEntityToModel(kiloAmount))
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
            return new KiloAmountPagingDTO() { Paging = pagingResult, KiloAmounts = kiloAmount };
        }
        catch (Exception ex)
        {
            LoggerHelper.Instance.LogError(ex, "Error occurred while fetching all kiloAmount.");
            throw;
        }
    }

    public KiloAmountDTO GetKiloAmountById(int id)
    {
        try
        {
            var kiloAmountEntity = _dbKiloTaxiContext.KiloAmounts.FirstOrDefault(s => s.Id == id);
            return KiloAmountConverter.ConvertEntityToModel(kiloAmountEntity);
        }
        catch (Exception ex)
        {
            LoggerHelper.Instance.LogError(
                ex,
                $"Error occurred while fetching kiloAmount with Id: {id}"
            );
            throw;
        }
    }

    public KiloAmountDTO CreateKiloAmount(KiloAmountDTO kiloAmountDTO)
    {
        try
        {
            var kiloAmountEntity = new KiloAmount();

            KiloAmountConverter.ConvertModelToEntity(kiloAmountDTO, ref kiloAmountEntity);

            _dbKiloTaxiContext.KiloAmounts.Add(kiloAmountEntity);
            _dbKiloTaxiContext.SaveChanges();

            kiloAmountDTO.Id = kiloAmountEntity.Id;
            return kiloAmountDTO;
        }
        catch (Exception ex)
        {
            LoggerHelper.Instance.LogError(ex, "Error occurred while creating kiloAmount.");
            throw;
        }
    }

    public bool UpdateKiloAmount(KiloAmountDTO kiloAmountDTO)
    {
        try
        {
            var kiloAmountEntity = _dbKiloTaxiContext.KiloAmounts.FirstOrDefault(s =>
                s.Id == kiloAmountDTO.Id
            );
            if (kiloAmountEntity == null)
                return false;
            KiloAmountConverter.ConvertModelToEntity(kiloAmountDTO, ref kiloAmountEntity);
            _dbKiloTaxiContext.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            LoggerHelper.Instance.LogError(ex, "Error occurred while updating kiloAmount.");
            throw;
        }
    }

    public bool DeleteKiloAmount(int id)
    {
        try
        {
            var kiloAmountEntity = _dbKiloTaxiContext.KiloAmounts.FirstOrDefault(s => s.Id == id);
            if (kiloAmountEntity == null)
                return false;

            _dbKiloTaxiContext.KiloAmounts.Remove(kiloAmountEntity);
            _dbKiloTaxiContext.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            LoggerHelper.Instance.LogError(
                ex,
                $"Error occurred while deleting kiloAmount with Id: {id}"
            );
            throw;
        }
    }
}
