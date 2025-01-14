using KiloTaxi.Model.DTO.Response;

namespace KiloTaxi.Model.DTO;

public class WalletPagingDTO
{
    public PagingResult Paging { get; set; }
    public IEnumerable<WalletInfoDTO> Wallets { get; set; }
}
