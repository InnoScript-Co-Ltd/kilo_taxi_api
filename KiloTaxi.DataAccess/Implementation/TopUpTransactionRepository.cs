using System.Linq.Expressions;
using KiloTaxi.Converter;
using KiloTaxi.DataAccess.Interface;
using KiloTaxi.EntityFramework;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;

namespace KiloTaxi.DataAccess.Implementation
{
    public class TopUpTransactionRepository : ITopUpTransactionRepository
    {
        private readonly DbKiloTaxiContext _dbKiloTaxiContext;

        public TopUpTransactionRepository(DbKiloTaxiContext dbKiloTaxiContext)
        {
            _dbKiloTaxiContext = dbKiloTaxiContext;
        }

        public TopUpTransactionDTO CreateTopUpTransaction(TopUpTransactionDTO topUpTransactionDTO)
        {
            try
            {
                // Convert DTO to entity
                var topUpTransactionEntity = new TopUpTransaction();
               
                TopUpTransactionConverter.ConvertModelToEntity(topUpTransactionDTO, ref topUpTransactionEntity);

                // Add the transaction to the database
                _dbKiloTaxiContext.TopUpTransactions.Add(topUpTransactionEntity);
                _dbKiloTaxiContext.SaveChanges();

                // Set the generated ID back to the DTO
                topUpTransactionDTO.Id = topUpTransactionEntity.Id;
                if (!topUpTransactionEntity.TransactionScreenShoot.Contains("default.png"))
                {
                    topUpTransactionEntity.TransactionScreenShoot = "screenShoot/" + topUpTransactionDTO.Id + topUpTransactionEntity.TransactionScreenShoot;
                    _dbKiloTaxiContext.SaveChanges();
                }


                return topUpTransactionDTO;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(ex, "Error occurred while creating a top-up transaction.");
                throw;
            }
        }

        public TopUpTransactionPagingDTO GetAllTopUpTransactions(PageSortParam pageSortParam)
        {
            try
            {
                // Start with a queryable collection of transactions
                var query = _dbKiloTaxiContext.TopUpTransactions.AsQueryable();

                // Apply search filter if provided
                if (!string.IsNullOrEmpty(pageSortParam.SearchTerm))
                {
                    query = query.Where(t =>
                        t.Id.ToString().Contains(pageSortParam.SearchTerm) ||
                        t.Amount.ToString().Contains(pageSortParam.SearchTerm) ||
                        t.Status.Contains(pageSortParam.SearchTerm));
                }

                // Get total count before applying pagination
                int totalCount = query.Count();

                // Apply sorting
                if (!string.IsNullOrEmpty(pageSortParam.SortField))
                {
                    // Ensure the SortField is valid
                    var sortField = pageSortParam.SortField;
                    var sortDirection = pageSortParam.SortDir == SortDirection.ASC ? "OrderBy" : "OrderByDescending";

                    //try
                    //{
                    //    query = query.OrderByPropertyName(sortField, sortDirection);
                    //}
                    //catch (Exception ex)
                    //{
                    //    LoggerHelper.Instance.LogError(ex, $"Invalid sort field: {sortField}");
                    //    throw new ArgumentException("Invalid sort field provided.", nameof(pageSortParam.SortField));
                    //}
                }

                // Apply pagination
                if (pageSortParam.PageSize > 0)
                {
                    query = query.Skip((pageSortParam.CurrentPage - 1) * pageSortParam.PageSize)
                                 .Take(pageSortParam.PageSize);
                }

                // Convert to DTOs
                var transactions = query.Select(t => TopUpTransactionConverter.ConvertEntityToModel(t)).ToList();

                // Prepare paging result
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSortParam.PageSize);
                var pagingResult = new PagingResult
                {
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    PreviousPage = pageSortParam.CurrentPage > 1 ? pageSortParam.CurrentPage - 1 : (int?)null,
                    NextPage = pageSortParam.CurrentPage < totalPages ? pageSortParam.CurrentPage + 1 : (int?)null,
                    FirstRowOnPage = ((pageSortParam.CurrentPage - 1) * pageSortParam.PageSize) + 1,
                    LastRowOnPage = Math.Min(totalCount, pageSortParam.CurrentPage * pageSortParam.PageSize)
                };

                return new TopUpTransactionPagingDTO
                {
                    Paging = pagingResult,
                    TopUpTransactions = transactions
                };
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(ex, "Error occurred while fetching top-up transactions.");
                throw;
            }
        }
    }
 }
