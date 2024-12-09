using KiloTaxi.Model.DTO;

namespace KiloTaxi.DataAccess.Interface;

public interface ISosRepository
{
    SosDTO CreateSos(SosDTO sosDTO);

    SosPagingDTO  GetAllSosList(PageSortParam pageSortParam);
    
    bool UpdateSos(SosDTO sosDTO);
    
    SosDTO GetSosById(int id);

    bool DeleteSos(int id);
}