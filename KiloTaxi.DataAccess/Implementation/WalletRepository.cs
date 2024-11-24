using KiloTaxi.Converter;
using KiloTaxi.DataAccess.Interface;
using KiloTaxi.EntityFramework;
using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Logging;
using KiloTaxi.Model.DTO;

namespace KiloTaxi.DataAccess.Implementation;

public class WalletRepository : IWalletRepository
{
    private readonly DbKiloTaxiContext _dbContext;

    public WalletRepository(DbKiloTaxiContext dbContext)
    {
        _dbContext = dbContext;
    }

    public WalletDTO CreateWallet(WalletDTO walletDTO)
    {
        try
        {
            var walletEntity = new Wallet();
            DateTime createdDate = DateTime.Now;
            walletDTO.CreatedDate = createdDate;
            WalletConverter.ConvertModelToEntity(walletDTO, ref walletEntity);

            _dbContext.Wallets.Add(walletEntity);
            _dbContext.SaveChanges();

            walletDTO.Id = walletEntity.Id;
            return walletDTO;
        }
        catch (Exception ex)
        {
            LoggerHelper.Instance.LogError(ex, "Error occurred while creating wallet.");
            throw;
        }
    }

    public bool UpdateWallet(WalletDTO walletDTO)
    {
        try
        {
            var walletEntity = _dbContext.Wallets.FirstOrDefault(w => w.Id == walletDTO.Id);
            if (walletEntity == null) return false;
            walletDTO.CreatedDate = walletEntity.CreatedDate;
            DateTime updateDate = DateTime.Now;
            walletDTO.UpdateDate = updateDate;
            WalletConverter.ConvertModelToEntity(walletDTO, ref walletEntity);
            _dbContext.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            LoggerHelper.Instance.LogError(ex, "Error occurred while updating wallet.");
            throw;
        }
    }

    public WalletDTO GetWalletById(int id)
    {
        try
        {
            var walletEntity = _dbContext.Wallets.FirstOrDefault(w => w.Id == id);
            return WalletConverter.ConvertEntityToModel(walletEntity);
        }
        catch (Exception ex)
        {
            LoggerHelper.Instance.LogError(ex, $"Error occurred while fetching wallet with Id: {id}");
            throw;
        }
    }

    public IEnumerable<WalletDTO> GetAllWallets()
    {
        try
        {
            return _dbContext.Wallets
                .Select(wallet => WalletConverter.ConvertEntityToModel(wallet))
                .ToList();
        }
        catch (Exception ex)
        {
            LoggerHelper.Instance.LogError(ex, "Error occurred while fetching all wallets.");
            throw;
        }
    }

    public bool DeleteWallet(int id)
    {
        try
        {
            var walletEntity = _dbContext.Wallets.FirstOrDefault(w => w.Id == id);
            if (walletEntity == null) return false;

            _dbContext.Wallets.Remove(walletEntity);
            _dbContext.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            LoggerHelper.Instance.LogError(ex, $"Error occurred while deleting wallet with Id: {id}");
            throw;
        }
    }
}
