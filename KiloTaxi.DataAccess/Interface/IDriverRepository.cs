using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;

namespace KiloTaxi.DataAccess.Interface;

public interface IDriverRepository
{
    DriverInfoDTO DriverRegistration(DriverFormDTO driverFormDto);
    bool UpdateDriver(DriverFormDTO driverDTO);
    DriverInfoDTO GetDriverById(int id);
    
    DriverPagingDTO GetAllDrivers(PageSortParam pageSortParam);
    
    bool DeleteDriver(int id);
    
    Task<DriverInfoDTO> ValidateDriverCredentials(string email, string password);
    List<DriverInfoDTO> SearchNearbyOnlineDriver();

}