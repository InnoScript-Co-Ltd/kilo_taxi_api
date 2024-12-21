using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Model.DTO;

namespace KiloTaxi.DataAccess.Interface;

public interface ITravelRateRepository
{
    TravelRateDTO AddTravelRate(TravelRateDTO travelRateDto);
    bool UpdateTravelRate(TravelRateDTO travelRateDto);
    TravelRatePagingDTO GetAllTravelRate(PageSortParam pageSortParam);
    bool DeleteTravelRate(int id);
    TravelRateDTO GetTravelRate(int id);
}