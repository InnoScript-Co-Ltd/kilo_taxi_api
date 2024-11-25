using KiloTaxi.Model.DTO;

namespace KiloTaxi.DataAccess.Interface;

public interface IWalletUserMappingRepository
{
    WalletUserMappingDTO CreateWalletUserMapping(WalletUserMappingDTO walletUserMappingDTO);
}
