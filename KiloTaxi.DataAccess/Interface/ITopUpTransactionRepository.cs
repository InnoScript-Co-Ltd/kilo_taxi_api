using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;

namespace KiloTaxi.DataAccess.Interface
{
    public interface ITopUpTransactionRepository
    {
        TopUpTransactionInfoDTO CreateTopUpTransaction(TopUpTransactionFormDTO topUpTransactionFormDTO);
        ResponseDTO<TopUpTransactionPagingDTO> GetAllTopUpTransactions(PageSortParam pageSortParam);
    }
}
