using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;

namespace KiloTaxi.DataAccess.Interface;

public interface IExtraDemandRepository
{
    ResponseDTO<ExtraDemandPagingDTO> GetAllExtraDemand(PageSortParam pageSortParam);
    ExtraDemandInfoDTO CreateExtraDemand(ExtraDemandFormDTO extraDemandFormDTO);
    bool UpdateExtraDemand(ExtraDemandFormDTO extraDemandFormDTO);
    ExtraDemandInfoDTO GetExtraDemandById(int id);
    bool DeleteExtraDemand(int id);
}
