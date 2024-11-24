using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Model.DTO;

namespace KiloTaxi.Converter;

public static class WalletUserMappingConverter
{
    public static WalletUserMappingDTO ConvertEntityToModel(WalletUserMapping walletUserMappingEntity)
    {
        if (walletUserMappingEntity == null)
            throw new ArgumentNullException(nameof(walletUserMappingEntity));

        return new WalletUserMappingDTO
        {
            Id = walletUserMappingEntity.Id,
            UserId = walletUserMappingEntity.UserId,
            Balance = walletUserMappingEntity.Balance,
            WalletType = walletUserMappingEntity.WalletType,
            Status = walletUserMappingEntity.Status,
            WalletId = walletUserMappingEntity.WalletId,
            CreatedDate = walletUserMappingEntity.CreatedDate,
            UpdatedDate = walletUserMappingEntity.UpdatedDate
        };
    }

    public static void ConvertModelToEntity(WalletUserMappingDTO walletUserMappingDTO, ref WalletUserMapping walletUserMappingEntity)
    {
        if (walletUserMappingDTO == null)
            throw new ArgumentNullException(nameof(walletUserMappingDTO));

        walletUserMappingEntity.Id = walletUserMappingDTO.Id;
        walletUserMappingEntity.UserId = walletUserMappingDTO.UserId;
        walletUserMappingEntity.Balance = walletUserMappingDTO.Balance;
        walletUserMappingEntity.WalletType = walletUserMappingDTO.WalletType;
        walletUserMappingEntity.Status = walletUserMappingDTO.Status;
        walletUserMappingEntity.WalletId = walletUserMappingDTO.WalletId;
        walletUserMappingEntity.CreatedDate = walletUserMappingDTO.CreatedDate;
        walletUserMappingEntity.UpdatedDate = walletUserMappingDTO.UpdatedDate;
    }
}
