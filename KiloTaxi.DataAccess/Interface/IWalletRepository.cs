using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;

namespace KiloTaxi.DataAccess.Interface;

public interface IWalletRepository
{
    WalletInfoDTO CreateWallet(WalletFormDTO walletFormDTO);
    bool UpdateWallet(WalletFormDTO walletFormDTO);
    WalletInfoDTO GetWalletById(int id);
    ResponseDTO<WalletPagingDTO> GetAllWallets(PageSortParam pageSortParam);
    bool DeleteWallet(int id);
}
