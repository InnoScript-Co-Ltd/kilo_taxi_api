using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;

namespace KiloTaxi.DataAccess.Interface;

public interface IReasonRepository
{
    ResponseDTO<ReasonPagingDTO> GetAllReason(PageSortParam pageSortParam);
    ReasonInfoDTO CreateReason(ReasonFormDTO reasonFormDTO);
    bool UpdateReason(ReasonFormDTO reasonFormDTO);
    ReasonInfoDTO GetReasonById(int id);
    bool DeleteReason(int id);
}
