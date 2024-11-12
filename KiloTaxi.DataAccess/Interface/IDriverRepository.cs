using KiloTaxi.Model.DTO;

namespace KiloTaxi.DataAccess.Interface;

public interface IDriverRepository
{
    DriverDTO AddDriver(DriverDTO driverDTO);
    bool UpdateDriver(DriverDTO driverDTO);
    DriverDTO GetDriverById(int id);
}