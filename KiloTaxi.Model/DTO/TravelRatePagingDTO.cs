using KiloTaxi.Model.DTO.Response;

namespace KiloTaxi.Model.DTO;

public class TravelRatePagingDTO
{
    public PagingResult Paging { get; set; }
    public IEnumerable<TravelRateInfoDTO> TravelRates { get; set; }
}