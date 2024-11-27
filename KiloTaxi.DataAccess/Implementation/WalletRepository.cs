using System.Linq.Expressions;
using KiloTaxi.Converter;
using KiloTaxi.DataAccess.Interface;
using KiloTaxi.EntityFramework;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;

namespace KiloTaxi.DataAccess.Implementation;

public class WalletRepository : IWalletRepository
{
    private readonly DbKiloTaxiContext _dbContext;

    public WalletRepository(DbKiloTaxiContext dbContext)
    {
        _dbContext = dbContext;
    }

    public WalletDTO CreateWallet(WalletDTO walletDTO)
    {
        try
        {
            var walletEntity = new Wallet();
            DateTime createdDate = DateTime.Now;
            walletDTO.CreateDate = createdDate;
            WalletConverter.ConvertModelToEntity(walletDTO, ref walletEntity);

            _dbContext.Wallets.Add(walletEntity);
            _dbContext.SaveChanges();

            walletDTO.Id = walletEntity.Id;
            return walletDTO;
        }
        catch (Exception ex)
        {
            LoggerHelper.Instance.LogError(ex, "Error occurred while creating wallet.");
            throw;
        }
    }

    public bool UpdateWallet(WalletDTO walletDTO)
    {
        try
        {
            var walletEntity = _dbContext.Wallets.FirstOrDefault(w => w.Id == walletDTO.Id);
            if (walletEntity == null) return false;
            walletDTO.CreateDate = walletEntity.CreatedDate;
            DateTime updateDate = DateTime.Now;
            walletDTO.UpdateDate = updateDate;
            WalletConverter.ConvertModelToEntity(walletDTO, ref walletEntity);
            _dbContext.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            LoggerHelper.Instance.LogError(ex, "Error occurred while updating wallet.");
            throw;
        }
    }

    public WalletDTO GetWalletById(int id)
    {
        try
        {
            var walletEntity = _dbContext.Wallets.FirstOrDefault(w => w.Id == id);
            return WalletConverter.ConvertEntityToModel(walletEntity);
        }
        catch (Exception ex)
        {
            LoggerHelper.Instance.LogError(ex, $"Error occurred while fetching wallet with Id: {id}");
            throw;
        }
    }

    public WalletPagingDTO GetAllWallets(PageSortParam pageSortParam)
    {
      try
        {
            var query = _dbContext.Wallets.AsQueryable();
            if (!string.IsNullOrEmpty(pageSortParam.SearchTerm))
            {
                query = query.Where(p =>
                    p.WalletName.Contains(pageSortParam.SearchTerm));
            }

            int totalCount = query.Count();
            if (!string.IsNullOrEmpty(pageSortParam.SortField))
            {
                var param = Expression.Parameter(typeof(Wallet), "p");
                var property = Expression.Property(param, pageSortParam.SortField);
                var sortExpression = Expression.Lambda(property, param);

                string sortMethod =
                    pageSortParam.SortDir == SortDirection.ASC ? "OrderBy" : "OrderByDescending";
                var orderByMethod = typeof(Queryable)
                    .GetMethods()
                    .Single(m => m.Name == sortMethod && m.GetParameters().Length == 2)
                    .MakeGenericMethod(typeof(Wallet), property.Type);
                query =
                    (IQueryable<Wallet>)(
                        orderByMethod.Invoke(
                            _dbContext,
                            new object[] { query, sortExpression }
                        ) ?? Enumerable.Empty<Driver>().AsQueryable()
                    );
            }

            if (query.Count() > pageSortParam.PageSize)
            {
                query = query
                    .Skip((pageSortParam.CurrentPage - 1) * pageSortParam.PageSize)
                    .Take(pageSortParam.PageSize);
            }

            var wallets = query
                .Select(wallet => WalletConverter.ConvertEntityToModel(wallet))
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
            return new WalletPagingDTO() { Paging = pagingResult, Wallets = wallets };
        }
        catch (Exception ex)
        {
            LoggerHelper.Instance.LogError(ex, "Error occurred while fetching all wallets.");
            throw;
        }
    }

    public bool DeleteWallet(int id)
    {
        try
        {
            var walletEntity = _dbContext.Wallets.FirstOrDefault(w => w.Id == id);
            if (walletEntity == null) return false;

            _dbContext.Wallets.Remove(walletEntity);
            _dbContext.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            LoggerHelper.Instance.LogError(ex, $"Error occurred while deleting wallet with Id: {id}");
            throw;
        }
    }
}
