using KiloTaxi.Model.DTO;

namespace KiloTaxi.DataAccess.Interface;

public interface IExtraDemandRepository
{
    ExtraDemandPagingDTO GetAllExtraDemand(PageSortParam pageSortParam);
    ExtraDemandDTO CreateExtraDemand(ExtraDemandDTO extraDemandDTO);
    bool UpdateExtraDemand(ExtraDemandDTO extraDemandDTO);
    ExtraDemandDTO GetExtraDemandById(int id);
    bool DeleteExtraDemand(int id);
}
