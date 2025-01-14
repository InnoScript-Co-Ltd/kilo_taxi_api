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

namespace KiloTaxi.DataAccess.Implementation
{
    public class TopUpTransactionRepository : ITopUpTransactionRepository
    {
        private readonly DbKiloTaxiContext _dbKiloTaxiContext;
        private readonly IWalletTransactionRepository _walletTransactionRepository;

        public TopUpTransactionRepository(DbKiloTaxiContext dbContext, IWalletTransactionRepository walletTransactionRepository)
        {
            _dbKiloTaxiContext = dbContext;
            _walletTransactionRepository = walletTransactionRepository;
        }

        public TopUpTransactionInfoDTO CreateTopUpTransaction(TopUpTransactionFormDTO topUpTransactionFormDTO)
        {
            try
            {
                var walletUserMapping = _dbKiloTaxiContext.WalletUserMappings
                                      .FirstOrDefault(w => w.UserId == topUpTransactionFormDTO.UseId);

                if (walletUserMapping == null)
                {
                    throw new Exception("No wallet mapping found for the provided UserId.");
                }

                var topUpTransactionEntity = new TopUpTransaction();

                TopUpTransactionConverter.ConvertModelToEntity(topUpTransactionFormDTO, ref topUpTransactionEntity);

                _dbKiloTaxiContext.TopUpTransactions.Add(topUpTransactionEntity);
                _dbKiloTaxiContext.SaveChanges();

                topUpTransactionFormDTO.Id = topUpTransactionEntity.Id;

                if (topUpTransactionEntity.Status == "Success") 
                {
                    var walletTransactionDTO = new WalletTransactionDTO
                    {
                        Amount = topUpTransactionEntity.Amount,
                        TransactionType = TransactionType.TopUp,
                        ReferenceId = topUpTransactionEntity.Id,
                        WalletUserMappingId = walletUserMapping.Id
                    };

                    _walletTransactionRepository.CreateWalletTransaction(walletTransactionDTO);
                }
                if (!string.IsNullOrEmpty(topUpTransactionEntity.TransactionScreenShoot) &&
                !topUpTransactionEntity.TransactionScreenShoot.Contains("default.png"))
                {
                    topUpTransactionEntity.TransactionScreenShoot = $"screenShoot/{topUpTransactionFormDTO.Id}{topUpTransactionEntity.TransactionScreenShoot}";
                    _dbKiloTaxiContext.SaveChanges();
                }
                
                var topUpTransactionInfoDTO = TopUpTransactionConverter.ConvertEntityToModel(topUpTransactionEntity);
                return topUpTransactionInfoDTO;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(ex, "Error occurred while creating a top-up transaction.");
                throw;
            }
        }

        public ResponseDTO<TopUpTransactionPagingDTO> GetAllTopUpTransactions(PageSortParam pageSortParam)
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
                
                ResponseDTO<TopUpTransactionPagingDTO> responseDto = new ResponseDTO<TopUpTransactionPagingDTO>();
                responseDto.StatusCode = (int)HttpStatusCode.OK;
                responseDto.Message = "Topup Transactions retrieved successfully";
                responseDto.TimeStamp = DateTime.Now;
                responseDto.Payload = new TopUpTransactionPagingDTO { Paging = pagingResult, TopUpTransactions = transactions };
                
                return responseDto;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(ex, "Error occurred while fetching top-up transactions.");
                throw;
            }
        }
    }
}
