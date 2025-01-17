using KiloTaxi.Model.DTO.Response;

namespace KiloTaxi.Model.DTO;

public class SosPagingDTO
{
    public PagingResult Paging { get; set; }
    public IEnumerable<SosInfoDTO> Sos { get; set; }
}