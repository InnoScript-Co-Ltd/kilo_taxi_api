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
        private string _mediaHostUrl;
        private readonly IWalletTransactionRepository _walletTransactionRepository;

        public TopUpTransactionRepository(
            DbKiloTaxiContext dbContext,
            IWalletTransactionRepository walletTransactionRepository
        )
        {
            _dbKiloTaxiContext = dbContext;
            _walletTransactionRepository = walletTransactionRepository;
        }

        public ResponseDTO<TopUpTransactionPagingDTO> GetAllTopUpTransactions(
            PageSortParam pageSortParam
        )
        {
            try
            {
                var query = _dbKiloTaxiContext.TopUpTransactions.AsQueryable();

                // Apply search filter if provided
                if (!string.IsNullOrEmpty(pageSortParam.SearchTerm))
                {
                    query = query.Where(t =>
                        t.Id.ToString().Contains(pageSortParam.SearchTerm)
                        || t.Amount.ToString().Contains(pageSortParam.SearchTerm)
                        || t.Status.Contains(pageSortParam.SearchTerm)
                    );
                }

                // Get total count before applying pagination
                int totalCount = query.Count();

                // Apply sorting
                if (!string.IsNullOrEmpty(pageSortParam.SortField))
                {
                    var param = Expression.Parameter(typeof(TopUpTransaction), "t");
                    var property = Expression.Property(param, pageSortParam.SortField);
                    var sortExpression = Expression.Lambda(property, param);

                    string sortMethod =
                        pageSortParam.SortDir == SortDirection.ASC
                            ? "OrderBy"
                            : "OrderByDescending";
                    var orderByMethod = typeof(Queryable)
                        .GetMethods()
                        .Single(m => m.Name == sortMethod && m.GetParameters().Length == 2)
                        .MakeGenericMethod(typeof(TopUpTransaction), property.Type);
                    query =
                        (IQueryable<TopUpTransaction>)(
                            orderByMethod.Invoke(
                                _dbKiloTaxiContext,
                                new object[] { query, sortExpression }
                            ) ?? Enumerable.Empty<TopUpTransaction>().AsQueryable()
                        );
                }

                // Apply pagination
                if (query.Count() > pageSortParam.PageSize)
                {
                    query = query
                        .Skip((pageSortParam.CurrentPage - 1) * pageSortParam.PageSize)
                        .Take(pageSortParam.PageSize);
                }

                // Convert to DTOs
                var transactions = query
                    .Select(t => TopUpTransactionConverter.ConvertEntityToModel(t, _mediaHostUrl))
                    .ToList();

                // Prepare paging result
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

                // Return the response DTO
                ResponseDTO<TopUpTransactionPagingDTO> responseDto =
                    new ResponseDTO<TopUpTransactionPagingDTO>
                    {
                        StatusCode = (int)HttpStatusCode.OK,
                        Message = "Top-up transactions retrieved successfully",
                        TimeStamp = DateTime.Now,
                        Payload = new TopUpTransactionPagingDTO
                        {
                            Paging = pagingResult,
                            TopUpTransactions = transactions,
                        },
                    };

                return responseDto;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    "Error occurred while fetching top-up transactions."
                );
                throw;
            }
        }

        public TopUpTransactionInfoDTO CreateTopUpTransaction(
            TopUpTransactionFormDTO topUpTransactionFormDTO
        )
        {
            try
            {
                // Retrieve the wallet mapping for the given user
                var walletUserMapping = _dbKiloTaxiContext.WalletUserMappings.FirstOrDefault(w =>
                    w.UserId == topUpTransactionFormDTO.UseId
                );

                if (walletUserMapping == null)
                {
                    throw new Exception("No wallet mapping found for the provided UserId.");
                }

                var topUpTransactionEntity = new TopUpTransaction();

                // Convert form DTO to entity
                TopUpTransactionConverter.ConvertModelToEntity(
                    topUpTransactionFormDTO,
                    ref topUpTransactionEntity
                );

                // Add top-up transaction to the database
                _dbKiloTaxiContext.TopUpTransactions.Add(topUpTransactionEntity);
                _dbKiloTaxiContext.SaveChanges();

                topUpTransactionFormDTO.Id = topUpTransactionEntity.Id;

                // Handle wallet transaction if the status is "Success"
                if (topUpTransactionEntity.Status == "Success")
                {
                    var walletTransactionDTO = new WalletTransactionDTO
                    {
                        Amount = topUpTransactionEntity.Amount,
                        TransactionType = TransactionType.TopUp,
                        ReferenceId = topUpTransactionEntity.Id,
                        WalletUserMappingId = walletUserMapping.Id,
                    };

                    _walletTransactionRepository.CreateWalletTransaction(walletTransactionDTO);
                }

                // Handle the transaction screenshot if applicable
                if (
                    !string.IsNullOrEmpty(topUpTransactionEntity.TransactionScreenShoot)
                    && !topUpTransactionEntity.TransactionScreenShoot.Contains("default.png")
                )
                {
                    topUpTransactionEntity.TransactionScreenShoot =
                        $"screenShoot/{topUpTransactionFormDTO.Id}{topUpTransactionEntity.TransactionScreenShoot}";
                    _dbKiloTaxiContext.SaveChanges();
                }

                // Convert the entity to InfoDTO and return
                var topUpTransactionInfoDTO = TopUpTransactionConverter.ConvertEntityToModel(
                    topUpTransactionEntity,
                    _mediaHostUrl
                );
                return topUpTransactionInfoDTO;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    "Error occurred while creating a top-up transaction."
                );
                throw;
            }
        }
    }
}
