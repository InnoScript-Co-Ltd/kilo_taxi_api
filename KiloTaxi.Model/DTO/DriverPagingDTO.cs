namespace KiloTaxi.Model.DTO;

public class DriverPagingDTO
{
    public PagingResult Paging { get; set; }
    public IEnumerable<DriverDTO> Drivers { get; set; }
}