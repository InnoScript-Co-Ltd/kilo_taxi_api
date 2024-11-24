using KiloTaxi.DataAccess.Interface;
using KiloTaxi.EntityFramework;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;
using KiloTaxi.Converter;

namespace KiloTaxi.DataAccess.Implementation;

public class WalletUserMappingRepository : IWalletUserMappingRepository
{
    private readonly DbKiloTaxiContext _dbContext;

    public WalletUserMappingRepository(DbKiloTaxiContext dbContext)
    {
        _dbContext = dbContext;
    }

    public WalletUserMappingDTO CreateWalletUserMapping(WalletUserMappingDTO walletUserMappingDTO)
    {
        try
        {
            var walletUserMappingEntity = new WalletUserMapping();
            DateTime createdDate = DateTime.Now;
            walletUserMappingDTO.CreatedDate = createdDate;
            WalletUserMappingConverter.ConvertModelToEntity(walletUserMappingDTO, ref walletUserMappingEntity);

            _dbContext.Set<WalletUserMapping>().Add(walletUserMappingEntity);
            _dbContext.SaveChanges();

            return WalletUserMappingConverter.ConvertEntityToModel(walletUserMappingEntity);
        }
        catch (Exception ex)
        {
            LoggerHelper.Instance.LogError(ex, "Error occurred while creating WalletUserMapping.");
            throw;
        }
    }
}
