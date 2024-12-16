using KiloTaxi.Model.DTO;

namespace KiloTaxi.DataAccess.Interface;

public interface ITransactionLogRepository
{
    TransactionLogPagingDTO GetAllTransactionLog(PageSortParam pageSortParam);
    TransactionLogDTO CreateTransactionLog(TransactionLogDTO transactionLogDTO);
    TransactionLogDTO GetTransactionLogByID(int id);
}
