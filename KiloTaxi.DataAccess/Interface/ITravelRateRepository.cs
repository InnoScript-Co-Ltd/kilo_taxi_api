using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;

namespace KiloTaxi.DataAccess.Interface;

public interface ITravelRateRepository
{
    TravelRateInfoDTO AddTravelRate(TravelRateFormDTO travelRateFormDto);
    bool UpdateTravelRate(TravelRateFormDTO travelRateFormDto);
    ResponseDTO<TravelRatePagingDTO> GetAllTravelRate(PageSortParam pageSortParam);
    bool DeleteTravelRate(int id);
    TravelRateInfoDTO GetTravelRate(int id);
}