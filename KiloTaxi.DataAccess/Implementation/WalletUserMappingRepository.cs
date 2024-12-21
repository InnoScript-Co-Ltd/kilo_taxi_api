using KiloTaxi.DataAccess.Interface;
using KiloTaxi.EntityFramework;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using KiloTaxi.Converter;

namespace KiloTaxi.DataAccess.Implementation;

public class WalletUserMappingRepository : IWalletUserMappingRepository
{
    private readonly DbKiloTaxiContext _dbKiloTaxiContext;

    public WalletUserMappingRepository(DbKiloTaxiContext dbContext)
    {
        _dbKiloTaxiContext = dbContext;
    }

    public WalletUserMappingDTO CreateWalletUserMapping(WalletUserMappingDTO walletUserMappingDTO)
    {
        try
        {
            bool userExists = _dbKiloTaxiContext.Set<WalletUserMapping>()
                                    .Any(w => w.UserId == walletUserMappingDTO.UserId);

            if (userExists)
            {
                throw new InvalidOperationException("A wallet mapping for the provided UserId already exists.");
            }

            var walletUserMappingEntity = new WalletUserMapping();
            DateTime createdDate = DateTime.Now;
            walletUserMappingDTO.CreatedDate = createdDate;
            WalletUserMappingConverter.ConvertModelToEntity(walletUserMappingDTO, ref walletUserMappingEntity);

            _dbKiloTaxiContext.Set<WalletUserMapping>().Add(walletUserMappingEntity);
            _dbKiloTaxiContext.SaveChanges();

            return WalletUserMappingConverter.ConvertEntityToModel(walletUserMappingEntity);
        }
        catch (Exception ex)
        {
            LoggerHelper.Instance.LogError(ex, "Error occurred while creating WalletUserMapping.");
            throw;
        }
    }
}
