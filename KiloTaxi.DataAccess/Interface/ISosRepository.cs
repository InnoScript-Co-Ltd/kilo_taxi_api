using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;

namespace KiloTaxi.DataAccess.Interface;

public interface ISosRepository
{
    SosInfoDTO CreateSos(SosFormDTO sosFormDTO);

    ResponseDTO<SosPagingDTO> GetAllSosList(PageSortParam pageSortParam);
    
    bool UpdateSos(SosFormDTO sosFormDTO);
    
    SosInfoDTO GetSosById(int id);

    bool DeleteSos(int id);
}