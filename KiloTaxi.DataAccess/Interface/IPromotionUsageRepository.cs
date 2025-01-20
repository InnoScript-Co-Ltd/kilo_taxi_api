using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;

namespace KiloTaxi.DataAccess.Interface;

public interface IPromotionUsageRepository
{
    ResponseDTO<PromotionUsagePagingDTO> GetAllPromotionUsage(PageSortParam pageSortParam);

    PromotionUsageInfoDTO AddPromotionUsage(PromotionUsageFormDTO promotionUsageFormDTO);
    bool UpdatePromotionUsage(PromotionUsageFormDTO promotionUsageFormDTO);
    PromotionUsageInfoDTO GetPromotionUsageById(int id);
    bool DeletePromotionUsage(int id);
    
    PromotionUsageInfoDTO findByCustomerId(int customerId);
}
