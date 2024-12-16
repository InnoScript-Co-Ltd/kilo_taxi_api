namespace KiloTaxi.Model.DTO;

public class CityPagingDTO
{
    public PagingResult Paging {  get; set; }
    public IEnumerable<CityDTO> Cities { get; set; }
}