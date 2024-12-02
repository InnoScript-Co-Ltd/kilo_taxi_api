using System.Linq;
using KiloTaxi.Common.ConfigurationSettings;
using KiloTaxi.Converter;
using KiloTaxi.DataAccess.Interface;
using KiloTaxi.EntityFramework;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using Microsoft.Extensions.Options;
using System.Linq.Expressions;
using KiloTaxi.Common.Enums;
using Microsoft.EntityFrameworkCore;

namespace KiloTaxi.DataAccess.Implementation
{
    public class WalletTransactionRepository : IWalletTransactionRepository
    {
        private readonly DbKiloTaxiContext _dbKiloTaxiContext;
        private string _mediaHostUrl;

        public WalletTransactionRepository(DbKiloTaxiContext dbKiloTaxiContext, IOptions<MediaSettings> mediaSettings)
        {
            _dbKiloTaxiContext = dbKiloTaxiContext;
            _mediaHostUrl = mediaSettings.Value.MediaHostUrl;
        }

        public WalletTransactionDTO CreateWalletTransaction(WalletTransactionDTO walletTransactionDTO)
        {
            try
            {
                var walletTransactionEntity = new WalletTransaction();
                DateTime TransactionDate = DateTime.Now;
                walletTransactionDTO.TransactionDate = TransactionDate;
                var walletUserMapping = _dbKiloTaxiContext.WalletUserMappings.FirstOrDefault(w => w.Id == walletTransactionDTO.WalletUserMappingId);
                walletTransactionDTO.BalanceBefore = walletUserMapping.Balance;
                if (walletTransactionDTO.TransactionType == TransactionType.TopUp)
                {
                    var topUpTransaction = _dbKiloTaxiContext.TopUpTransactions.FirstOrDefault(t => t.Id == walletTransactionDTO.ReferenceId);
                    walletTransactionDTO.BalanceAfter = walletTransactionDTO.BalanceBefore + topUpTransaction.Amount;
                }
                else if (walletTransactionDTO.TransactionType == TransactionType.Order)
                {
                    var order = _dbKiloTaxiContext.Orders.FirstOrDefault(t => t.Id == walletTransactionDTO.ReferenceId);
                    walletTransactionDTO.BalanceAfter = walletTransactionDTO.BalanceBefore + order.TotalAmount;
                }
                else if (walletTransactionDTO.TransactionType == TransactionType.PromotionUsage){ 
                    var promotionUsage = _dbKiloTaxiContext.PromotionUsages.FirstOrDefault(p => p.Id == walletTransactionDTO.ReferenceId);
                    walletTransactionDTO.BalanceAfter = walletTransactionDTO.BalanceBefore - promotionUsage.DiscountApplied;
                }
                else
                {
                    var errorMessage = $"TransactionType '{walletTransactionDTO.TransactionType}' is not recognized.";
                    LoggerHelper.Instance.LogError(errorMessage);
                    throw new InvalidOperationException(errorMessage);
                }
                WalletTransactionConverter.ConvertModelToEntity(walletTransactionDTO, ref walletTransactionEntity);

                _dbKiloTaxiContext.Add(walletTransactionEntity);
                _dbKiloTaxiContext.SaveChanges();

                walletTransactionDTO.Id = walletTransactionEntity.Id;

                walletTransactionDTO = WalletTransactionConverter.ConvertEntityToModel(walletTransactionEntity);
                return walletTransactionDTO;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(ex, "Error occurred while creating wallet transaction.");
                throw;
            }
        }

        public bool UpdateWalletTransaction(WalletTransactionDTO walletTransactionDTO)
        {
            bool result = false;
            try
            {
                var walletTransactionEntity = _dbKiloTaxiContext.WalletTransactions
                    .FirstOrDefault(wt => wt.Id == walletTransactionDTO.Id);

                if (walletTransactionEntity == null)
                {
                    return result;
                }

                WalletTransactionConverter.ConvertModelToEntity(walletTransactionDTO, ref walletTransactionEntity);
                _dbKiloTaxiContext.SaveChanges();
                result = true;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(ex, $"Error occurred while updating wallet transaction with Id: {walletTransactionDTO.Id}");
                throw;
            }

            return result;
        }

        public WalletTransactionDTO GetWalletTransactionById(int id)
        {
            try
            {
                var walletTransactionEntity = _dbKiloTaxiContext.WalletTransactions
                    .FirstOrDefault(wt => wt.Id == id);

                if (walletTransactionEntity == null)
                {
                    return null;
                }

                var walletTransactionDTO = WalletTransactionConverter.ConvertEntityToModel(walletTransactionEntity);
                return walletTransactionDTO;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(ex, $"Error occurred while fetching wallet transaction with Id: {id}");
                throw;
            }
        }

        public List<WalletTransactionDTO> GetAllWalletTransactions(PageSortParam pageSortParam)
        {
            try
            {
                var query = _dbKiloTaxiContext.WalletTransactions.AsQueryable();

                if (!string.IsNullOrEmpty(pageSortParam.SearchTerm))
                {
                    query = query.Where(wt => wt.Details.Contains(pageSortParam.SearchTerm));
                }

                int totalCount = query.Count();

                if (!string.IsNullOrEmpty(pageSortParam.SortField))
                {
                    var param = Expression.Parameter(typeof(WalletTransaction), "wt");
                    var property = Expression.Property(param, pageSortParam.SortField);
                    var sortExpression = Expression.Lambda(property, param);

                    string sortMethod = pageSortParam.SortDir == SortDirection.ASC ? "OrderBy" : "OrderByDescending";
                    var orderByMethod = typeof(Queryable).GetMethods()
                        .Single(m => m.Name == sortMethod && m.GetParameters().Length == 2)
                        .MakeGenericMethod(typeof(WalletTransaction), property.Type);

                    query = (IQueryable<WalletTransaction>)orderByMethod.Invoke(_dbKiloTaxiContext, new object[] { query, sortExpression }) ?? Enumerable.Empty<WalletTransaction>().AsQueryable();
                }

                if (query.Count() > pageSortParam.PageSize)
                {
                    query = query.Skip((pageSortParam.CurrentPage - 1) * pageSortParam.PageSize)
                                 .Take(pageSortParam.PageSize);
                }

                var walletTransactions = query.Select(wt => WalletTransactionConverter.ConvertEntityToModel(wt)).ToList();
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSortParam.PageSize);

                return walletTransactions;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(ex, "Error occurred while fetching all wallet transactions.");
                throw;
            }
        }

        public bool DeleteWalletTransaction(int id)
        {
            bool result = false;
            try
            {
                var walletTransactionEntity = _dbKiloTaxiContext.WalletTransactions
                    .FirstOrDefault(wt => wt.Id == id);

                if (walletTransactionEntity == null)
                {
                    return result;
                }

                _dbKiloTaxiContext.WalletTransactions.Remove(walletTransactionEntity);
                _dbKiloTaxiContext.SaveChanges();
                result = true;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(ex, $"Error occurred while deleting wallet transaction with Id: {id}");
                throw;
            }

            return result;
        }
    }
}
