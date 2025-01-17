using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;

namespace KiloTaxi.DataAccess.Interface;

public interface IReviewRepository
{
    ResponseDTO<ReviewPagingDTO> GetAllReview(PageSortParam pageSortParam);

    ReviewInfoDTO AddReview(ReviewFormDTO reviewFormDTO);
    bool UpdateReview(ReviewFormDTO reviewFormDTO);
    ReviewInfoDTO GetReviewById(int id);
    bool DeleteReview(int id);
}
