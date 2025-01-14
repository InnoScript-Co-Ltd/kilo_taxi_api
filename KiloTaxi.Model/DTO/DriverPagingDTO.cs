using KiloTaxi.Model.DTO.Response;

namespace KiloTaxi.Model.DTO;

public class DriverPagingDTO
{
    public PagingResult Paging { get; set; }
    public IEnumerable<DriverInfoDTO> Drivers { get; set; }
}
