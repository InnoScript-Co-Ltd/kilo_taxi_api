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
    public class ReviewRepository : IReviewRepository
    {
        private readonly DbKiloTaxiContext _dbKiloTaxiContext;

        public ReviewRepository(DbKiloTaxiContext dbContext)
        {
            _dbKiloTaxiContext = dbContext;
        }

        public ResponseDTO<ReviewPagingDTO> GetAllReview(PageSortParam pageSortParam)
        {
            try
            {
                var query = _dbKiloTaxiContext
                    .Reviews.Include(r => r.Customer)
                    .Include(r => r.Driver)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(pageSortParam.SearchTerm))
                {
                    query = query.Where(review =>
                        review.ReviewContent.Contains(pageSortParam.SearchTerm)
                        || review.Customer.Name.Contains(pageSortParam.SearchTerm)
                        || review.Driver.Name.Contains(pageSortParam.SearchTerm)
                    );
                }

                int totalCount = query.Count();

                if (!string.IsNullOrEmpty(pageSortParam.SortField))
                {
                    var param = Expression.Parameter(typeof(Review), "review");
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
                        .MakeGenericMethod(typeof(Review), property.Type);

                    query =
                        (IQueryable<Review>)
                            orderByMethod.Invoke(null, new object[] { query, sortExpression });
                }

                if (query.Count() > pageSortParam.PageSize)
                {
                    query = query
                        .Skip((pageSortParam.CurrentPage - 1) * pageSortParam.PageSize)
                        .Take(pageSortParam.PageSize);
                }

                var reviews = query.Select(ReviewConverter.ConvertEntityToModel).ToList();

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

                ResponseDTO<ReviewPagingDTO> responseDto = new ResponseDTO<ReviewPagingDTO>();
                responseDto.StatusCode = (int)HttpStatusCode.OK;
                responseDto.Message = "reviews retrieved successfully";
                responseDto.TimeStamp = DateTime.Now;
                responseDto.Payload = new ReviewPagingDTO { Paging = pagingResult, Reviews = reviews };
                return responseDto;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(ex, "Error occurred while fetching all reviews.");
                throw;
            }
        }

        public ReviewInfoDTO AddReview(ReviewFormDTO reviewFormDTO)
        {
            try
            {
                Review reviewEntity = new Review();
                ReviewConverter.ConvertModelToEntity(reviewFormDTO, ref reviewEntity);

                var customer = _dbKiloTaxiContext.Customers.FirstOrDefault(c =>
                    c.Id == reviewFormDTO.CustomerId
                );
                var driver = _dbKiloTaxiContext.Drivers.FirstOrDefault(s =>
                    s.Id == reviewFormDTO.DriverId
                );

                _dbKiloTaxiContext.Add(reviewEntity);
                _dbKiloTaxiContext.SaveChanges();

                reviewFormDTO.Id = reviewEntity.Id;
                reviewFormDTO.CustomerName = customer.Name;
                reviewFormDTO.DriverName = driver.Name;

                LoggerHelper.Instance.LogInfo(
                    $"Review added successfully with Id: {reviewEntity.Id}"
                );

                var reviewInfoDTO = ReviewConverter.ConvertEntityToModel(reviewEntity);
                return reviewInfoDTO;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(ex, "Error occurred while adding review.");
                throw;
            }
        }

        public bool UpdateReview(ReviewFormDTO reviewFormDTO)
        {
            try
            {
                var reviewEntity = _dbKiloTaxiContext.Reviews.FirstOrDefault(review =>
                    review.Id == reviewFormDTO.Id
                );
                if (reviewEntity == null)
                {
                    return false;
                }

                ReviewConverter.ConvertModelToEntity(reviewFormDTO, ref reviewEntity);
                _dbKiloTaxiContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    $"Error occurred while updating review with Id: {reviewFormDTO.Id}"
                );
                throw;
            }
        }

        public ReviewInfoDTO GetReviewById(int id)
        {
            try
            {
                var reviewEntity = _dbKiloTaxiContext
                    .Reviews.Include(r => r.Customer)
                    .Include(r => r.Driver)
                    .FirstOrDefault(review => review.Id == id);

                if (reviewEntity == null)
                {
                    LoggerHelper.Instance.LogError($"Review with Id: {id} not found.");
                    return null;
                }

                return ReviewConverter.ConvertEntityToModel(reviewEntity);
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    $"Error occurred while fetching review with Id: {id}"
                );
                throw;
            }
        }

        public bool DeleteReview(int id)
        {
            try
            {
                var reviewEntity = _dbKiloTaxiContext.Reviews.FirstOrDefault(review =>
                    review.Id == id
                );
                if (reviewEntity == null)
                {
                    return false;
                }

                _dbKiloTaxiContext.Reviews.Remove(reviewEntity);
                _dbKiloTaxiContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    $"Error occurred while deleting review with Id: {id}"
                );
                throw;
            }
        }
    }
}
