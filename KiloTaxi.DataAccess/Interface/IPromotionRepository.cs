using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;

namespace KiloTaxi.DataAccess.Interface;

public interface IPromotionRepository
{
    ResponseDTO<PromotionPagingDTO> GetAllPromotion(PageSortParam pageSortParam);

    PromotionInfoDTO AddPromotion(PromotionFormDTO promotionFormDTO);
    bool UpdatePromotion(PromotionFormDTO promotionFormDTO);
    PromotionInfoDTO GetPromotionById(int id);
    bool DeletePromotion(int id);
}
