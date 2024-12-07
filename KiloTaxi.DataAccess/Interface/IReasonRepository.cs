using KiloTaxi.Model.DTO;

namespace KiloTaxi.DataAccess.Interface;

public interface IReasonRepository
{
    ReasonPagingDTO GetAllReason(PageSortParam pageSortParam);
    ReasonDTO CreateReason(ReasonDTO reasonDTO);
    bool UpdateReason(ReasonDTO reasonDTO);
    ReasonDTO GetReasonById(int id);
    bool DeleteReason(int id);
}
