using KiloTaxi.Converter;
using KiloTaxi.DataAccess.Interface;
using KiloTaxi.EntityFramework;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace KiloTaxi.DataAccess.Implementation;

public class PaymentChannelRepository : IPaymentChannelRepository
{
    private readonly DbKiloTaxiContext _dbContext;

    public PaymentChannelRepository(DbKiloTaxiContext dbContext)
    {
        _dbContext = dbContext;
    }

    public PaymentChannelDTO CreatePaymentChannel(PaymentChannelDTO paymentChannelDTO)
    {
        try
        {
            var paymentChannelEntity = new PaymentChannel();
            PaymentChannelConverter.ConvertModelToEntity(paymentChannelDTO, ref paymentChannelEntity);

            _dbContext.PaymentChannels.Add(paymentChannelEntity);
            _dbContext.SaveChanges();

            paymentChannelDTO.Id = paymentChannelEntity.Id;
            return paymentChannelDTO;
        }
        catch (Exception ex)
        {
            LoggerHelper.Instance.LogError(ex, "Error occurred while creating payment channel.");
            throw;
        }
    }

    public bool UpdatePaymentChannel(PaymentChannelDTO paymentChannelDTO)
    {
        try
        {
            var paymentChannelEntity =
                _dbContext.PaymentChannels.FirstOrDefault(pc => pc.Id == paymentChannelDTO.Id);
            if (paymentChannelEntity == null) return false;

            PaymentChannelConverter.ConvertModelToEntity(paymentChannelDTO, ref paymentChannelEntity);
            _dbContext.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            LoggerHelper.Instance.LogError(ex, "Error occurred while updating payment channel.");
            throw;
        }
    }

    public PaymentChannelDTO GetPaymentChannelById(int id)
    {
        try
        {
            var paymentChannelEntity = _dbContext.PaymentChannels.FirstOrDefault(pc => pc.Id == id);
            return PaymentChannelConverter.ConvertEntityToModel(paymentChannelEntity);
        }
        catch (Exception ex)
        {
            LoggerHelper.Instance.LogError(ex, $"Error occurred while fetching payment channel with Id: {id}");
            throw;
        }
    }

    public PaymentChannelPagingDTO GetAllPaymentChannels(PageSortParam pageSortParam)
    {
        try
        {
            var query = _dbContext.PaymentChannels.AsQueryable();

            if (!string.IsNullOrEmpty(pageSortParam.SearchTerm))
            {
                query = query.Where(p =>
                    p.ChannelName.Contains(pageSortParam.SearchTerm) ||
                    p.Description.Contains(pageSortParam.SearchTerm));
            }

            int totalCount = query.Count();

            if (!string.IsNullOrEmpty(pageSortParam.SortField))
            {
                var param = Expression.Parameter(typeof(PaymentChannel), "p");
                var property = Expression.Property(param, pageSortParam.SortField);
                var sortExpression = Expression.Lambda(property, param);

                string sortMethod = pageSortParam.SortDir == SortDirection.ASC ? "OrderBy" : "OrderByDescending";
                var orderByMethod = typeof(Queryable)
                    .GetMethods()
                    .Single(m => m.Name == sortMethod && m.GetParameters().Length == 2)
                    .MakeGenericMethod(typeof(PaymentChannel), property.Type);
                query =
                    (IQueryable<PaymentChannel>)(orderByMethod.Invoke(
                        null,
                        new object[] { query, sortExpression }
                    ) ?? Enumerable.Empty<PaymentChannel>().AsQueryable());
            }

            if (query.Count() > pageSortParam.PageSize)
            {
                query = query
                    .Skip((pageSortParam.CurrentPage - 1) * pageSortParam.PageSize)
                    .Take(pageSortParam.PageSize);
            }

            var paymentChannels = query
                .Select(channel => PaymentChannelConverter.ConvertEntityToModel(channel))
                .ToList();

            var totalPages = (int)Math.Ceiling((double)totalCount / pageSortParam.PageSize);
            var pagingResult = new PagingResult
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                PreviousPage = pageSortParam.CurrentPage > 1 ? pageSortParam.CurrentPage - 1 : (int?)null,
                NextPage = pageSortParam.CurrentPage < totalPages ? pageSortParam.CurrentPage + 1 : (int?)null,
                FirstRowOnPage = ((pageSortParam.CurrentPage - 1) * pageSortParam.PageSize) + 1,
                LastRowOnPage = Math.Min(totalCount, pageSortParam.CurrentPage * pageSortParam.PageSize),
            };

            return new PaymentChannelPagingDTO
            {
                Paging = pagingResult,
                PaymentChannels = paymentChannels
            };
        }
        catch (Exception ex)
        {
            LoggerHelper.Instance.LogError(ex, "Error occurred while fetching all payment channels.");
            throw;
        }
    }




    public bool DeletePaymentChannel(int id)
    {
        try
        {
            var paymentChannelEntity = _dbContext.PaymentChannels.FirstOrDefault(pc => pc.Id == id);
            if (paymentChannelEntity == null) return false;

            _dbContext.PaymentChannels.Remove(paymentChannelEntity);
            _dbContext.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            LoggerHelper.Instance.LogError(ex, $"Error occurred while deleting payment channel with Id: {id}");
            throw;
        }
    }
}
