using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Model.DTO;

namespace KiloTaxi.Converter;

public static class WalletConverter
{
    public static WalletDTO ConvertEntityToModel(Wallet walletEntity)
    {
        if (walletEntity == null) throw new ArgumentNullException(nameof(walletEntity));

        return new WalletDTO
        {
            Id = walletEntity.Id,
            WalletName = walletEntity.WalletName,
            CreatedDate = walletEntity.CreatedDate,
            UpdateDate = walletEntity.UpdateDate
        };
    }

    public static void ConvertModelToEntity(WalletDTO walletDTO, ref Wallet walletEntity)
    {
        if (walletDTO == null) throw new ArgumentNullException(nameof(walletDTO));

        walletEntity.Id = walletDTO.Id;
        walletEntity.WalletName = walletDTO.WalletName;
        walletEntity.CreatedDate = walletDTO.CreatedDate;
        walletEntity.UpdateDate = walletDTO.UpdateDate;
    }
}
