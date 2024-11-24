namespace KiloTaxi.Model.DTO;

public class WalletPagingDTO
{
    public PagingResult Paging { get; set; }
    public IEnumerable<WalletDTO> Wallets { get; set; }
}
