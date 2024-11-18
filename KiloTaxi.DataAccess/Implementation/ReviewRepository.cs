using System.Linq.Expressions;
using KiloTaxi.Converter;
using KiloTaxi.DataAccess.Interface;
using KiloTaxi.EntityFramework;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
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

        public ReviewPagingDTO GetAllReview(PageSortParam pageSortParam)
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

                return new ReviewPagingDTO { Paging = pagingResult, Reviews = reviews };
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(ex, "Error occurred while fetching all reviews.");
                throw;
            }
        }

        public ReviewDTO AddReview(ReviewDTO reviewDTO)
        {
            try
            {
                Review reviewEntity = new Review();
                ReviewConverter.ConvertModelToEntity(reviewDTO, ref reviewEntity);

                var customer = _dbKiloTaxiContext.Customers.FirstOrDefault(c =>
                    c.Id == reviewDTO.CustomerId
                );
                var driver = _dbKiloTaxiContext.Drivers.FirstOrDefault(s =>
                    s.Id == reviewDTO.CustomerId
                );

                _dbKiloTaxiContext.Add(reviewEntity);
                _dbKiloTaxiContext.SaveChanges();

                reviewDTO.Id = reviewEntity.Id;
                reviewDTO.CustomerName = customer.Name;
                reviewDTO.DriverName = driver.Name;

                LoggerHelper.Instance.LogInfo(
                    $"Review added successfully with Id: {reviewEntity.Id}"
                );

                return reviewDTO;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(ex, "Error occurred while adding review.");
                throw;
            }
        }

        public bool UpdateReview(ReviewDTO reviewDTO)
        {
            try
            {
                var reviewEntity = _dbKiloTaxiContext.Reviews.FirstOrDefault(review =>
                    review.Id == reviewDTO.Id
                );
                if (reviewEntity == null)
                {
                    return false;
                }

                ReviewConverter.ConvertModelToEntity(reviewDTO, ref reviewEntity);
                _dbKiloTaxiContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.LogError(
                    ex,
                    $"Error occurred while updating review with Id: {reviewDTO.Id}"
                );
                throw;
            }
        }

        public ReviewDTO GetReviewById(int id)
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
