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
            WalletUserMappingConverter.ConvertModelToEntity(walletUserMappingDTO, ref walletUserMappingEntity);

            walletUserMappingEntity.CreatedDate = DateTime.UtcNow;

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

    public bool UpdateWalletUserMapping(WalletUserMappingDTO walletUserMappingDTO)
    {
        try
        {
            var walletUserMappingEntity = _dbContext.Set<WalletUserMapping>().FirstOrDefault(w => w.Id == walletUserMappingDTO.Id);
            if (walletUserMappingEntity == null)
                return false;

            walletUserMappingDTO.CreatedDate = walletUserMappingEntity.CreatedDate; // Preserve original CreatedDate
            walletUserMappingDTO.UpdatedDate = DateTime.UtcNow; // Update UpdatedDate

            WalletUserMappingConverter.ConvertModelToEntity(walletUserMappingDTO, ref walletUserMappingEntity);

            _dbContext.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            LoggerHelper.Instance.LogError(ex, "Error occurred while updating WalletUserMapping.");
            throw;
        }
    }

    public WalletUserMappingDTO GetWalletUserMappingById(int id)
    {
        try
        {
            var walletUserMappingEntity = _dbContext.Set<WalletUserMapping>().FirstOrDefault(w => w.Id == id);
            if (walletUserMappingEntity == null)
                return null;

            return WalletUserMappingConverter.ConvertEntityToModel(walletUserMappingEntity);
        }
        catch (Exception ex)
        {
            LoggerHelper.Instance.LogError(ex, $"Error occurred while fetching WalletUserMapping with ID {id}.");
            throw;
        }
    }

    public IEnumerable<WalletUserMappingDTO> GetAllWalletUserMappings()
    {
        try
        {
            return _dbContext.Set<WalletUserMapping>()
                .Select(walletUserMapping => WalletUserMappingConverter.ConvertEntityToModel(walletUserMapping))
                .ToList();
        }
        catch (Exception ex)
        {
            LoggerHelper.Instance.LogError(ex, "Error occurred while fetching all WalletUserMappings.");
            throw;
        }
    }

    public bool DeleteWalletUserMapping(int id)
    {
        try
        {
            var walletUserMappingEntity = _dbContext.Set<WalletUserMapping>().FirstOrDefault(w => w.Id == id);
            if (walletUserMappingEntity == null)
                return false;

            _dbContext.Set<WalletUserMapping>().Remove(walletUserMappingEntity);
            _dbContext.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            LoggerHelper.Instance.LogError(ex, $"Error occurred while deleting WalletUserMapping with ID {id}.");
            throw;
        }
    }
}
