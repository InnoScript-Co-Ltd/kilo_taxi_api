using KiloTaxi.Model.DTO;

namespace KiloTaxi.DataAccess.Interface;

public interface IVehicleRepository
{
    VehicleDTO VehicleRegistration(VehicleDTO vehicleDTO);
    bool UpdateVehicle(VehicleDTO vehicleDTO);
    VehicleDTO GetVehicleById(int id);
    
    VehiclePagingDTO GetAllVehicle(PageSortParam pageSortParam);
    
    bool DeleteVehicle(int id);
}