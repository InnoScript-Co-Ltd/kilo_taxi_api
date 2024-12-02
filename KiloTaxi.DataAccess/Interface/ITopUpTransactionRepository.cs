using KiloTaxi.Model.DTO;

namespace KiloTaxi.DataAccess.Interface
{
    public interface ITopUpTransactionRepository
    {
        TopUpTransactionDTO CreateTopUpTransaction(TopUpTransactionDTO topUpTransactionDTO);
        TopUpTransactionPagingDTO GetAllTopUpTransactions(PageSortParam pageSortParam);
    }
}
