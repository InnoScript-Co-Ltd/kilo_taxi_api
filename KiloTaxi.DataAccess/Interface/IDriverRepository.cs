using KiloTaxi.Model.DTO;

namespace KiloTaxi.DataAccess.Interface;

public interface IDriverRepository
{
    DriverDTO DriverRegistration(DriverDTO driverDTO);
    bool UpdateDriver(DriverDTO driverDTO);
    DriverDTO GetDriverById(int id);
    
    DriverPagingDTO GetAllDrivers(PageSortParam pageSortParam);
    
    bool DeleteDriver(int id);
}