using KiloTaxi.Model.DTO;

namespace KiloTaxi.DataAccess.Interface;

public interface IPromotionRepository
{
    PromotionPagingDTO GetAllPromotion(PageSortParam pageSortParam);

    PromotionDTO AddPromotion(PromotionDTO promotionDTO);
    bool UpdatePromotion(PromotionDTO promotionDTO);
    PromotionDTO GetPromotionById(int id);
    bool DeletePromotion(int id);
}
