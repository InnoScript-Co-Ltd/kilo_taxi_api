using KiloTaxi.Model.DTO;

namespace KiloTaxi.DataAccess.Interface;

public interface IPromotionUsageRepository
{
    PromotionUsagePagingDTO GetAllPromotionUsage(PageSortParam pageSortParam);

    PromotionUsageDTO AddPromotionUsage(PromotionUsageDTO promotionDTO);
    bool UpdatePromotionUsage(PromotionUsageDTO promotionDTO);
    PromotionUsageDTO GetPromotionUsageById(int id);
    bool DeletePromotionUsage(int id);
}
