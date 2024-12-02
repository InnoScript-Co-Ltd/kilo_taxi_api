using KiloTaxi.Model.DTO;

namespace KiloTaxi.DataAccess.Interface
{
    public interface IWalletTransactionRepository
    {
        WalletTransactionDTO CreateWalletTransaction(WalletTransactionDTO walletTransactionDTO);
        bool UpdateWalletTransaction(WalletTransactionDTO walletTransactionDTO);
        WalletTransactionDTO GetWalletTransactionById(int id);
        List<WalletTransactionDTO> GetAllWalletTransactions(PageSortParam pageSortParam);
        bool DeleteWalletTransaction(int id);
    }
}
