namespace KiloTaxi.Model.DTO;

public class KiloAmountPagingDTO
{
    public PagingResult Paging { get; set; }
    public IEnumerable<KiloAmountDTO> KiloAmounts { get; set; }
}
