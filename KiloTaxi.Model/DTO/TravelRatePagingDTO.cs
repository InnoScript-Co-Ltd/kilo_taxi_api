namespace KiloTaxi.Model.DTO;

public class TravelRatePagingDTO
{
    public PagingResult Paging { get; set; }
    public IEnumerable<TravelRateDTO> TravelRates { get; set; }
}