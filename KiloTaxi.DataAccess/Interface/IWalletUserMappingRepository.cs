using KiloTaxi.Model.DTO;

namespace KiloTaxi.DataAccess.Interface;

public interface IWalletUserMappingRepository
{
    WalletUserMappingDTO CreateWalletUserMapping(WalletUserMappingDTO walletUserMappingDTO);
    bool UpdateWalletUserMapping(WalletUserMappingDTO walletUserMappingDTO);
    WalletUserMappingDTO GetWalletUserMappingById(int id);
    IEnumerable<WalletUserMappingDTO> GetAllWalletUserMappings();
    bool DeleteWalletUserMapping(int id);
}
