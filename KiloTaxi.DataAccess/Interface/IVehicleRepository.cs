using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;

namespace KiloTaxi.DataAccess.Interface;

public interface IVehicleRepository
{
    VehicleInfoDTO VehicleRegistration(DriverCreateFormDTO vehicleDTO);
    bool UpdateVehicle(VehicleUpdateFormDTO vehicleDTO);
    VehicleInfoDTO GetVehicleById(int id);
    
    VehiclePagingDTO GetAllVehicle(PageSortParam pageSortParam);
    
    bool DeleteVehicle(int id);
}