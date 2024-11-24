using KiloTaxi.Model.DTO;

namespace KiloTaxi.DataAccess.Interface;

public interface IWalletRepository
{
    WalletDTO CreateWallet(WalletDTO walletDTO);
    bool UpdateWallet(WalletDTO walletDTO);
    WalletDTO GetWalletById(int id);
    IEnumerable<WalletDTO> GetAllWallets();
    bool DeleteWallet(int id);
}
