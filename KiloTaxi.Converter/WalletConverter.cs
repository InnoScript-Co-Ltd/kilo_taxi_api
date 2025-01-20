using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;

namespace KiloTaxi.Converter;

public static class WalletConverter
{
    public static WalletInfoDTO ConvertEntityToModel(Wallet walletEntity)
    {
        if (walletEntity == null) throw new ArgumentNullException(nameof(walletEntity));

        return new WalletInfoDTO
        {
            Id = walletEntity.Id,
            WalletName = walletEntity.WalletName,
            CreateDate = walletEntity.CreatedDate,
            UpdateDate = walletEntity.UpdateDate
        };
    }

    public static void ConvertModelToEntity(WalletFormDTO walletFormDTO, ref Wallet walletEntity)
    {
        if (walletFormDTO == null) throw new ArgumentNullException(nameof(walletFormDTO));

        walletEntity.Id = walletFormDTO.Id;
        walletEntity.WalletName = walletFormDTO.WalletName;
        walletEntity.CreatedDate = walletFormDTO.CreateDate;
        walletEntity.UpdateDate = walletFormDTO.UpdateDate;
    }
}
