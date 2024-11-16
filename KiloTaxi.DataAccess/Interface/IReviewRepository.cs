using KiloTaxi.Model.DTO;

namespace KiloTaxi.DataAccess.Interface;

public interface IReviewRepository
{
    ReviewPagingDTO GetAllReview(PageSortParam pageSortParam);

    ReviewDTO AddReview(ReviewDTO reviewDTO);
    bool UpdateReview(ReviewDTO reviewDTO);
    ReviewDTO GetReviewById(int id);
    bool DeleteReview(int id);
}
