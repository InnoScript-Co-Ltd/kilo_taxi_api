namespace KiloTaxi.Model.DTO;

public class SosPagingDTO
{
    public PagingResult Paging { get; set; }
    public IEnumerable<SosDTO> Sos { get; set; }
}