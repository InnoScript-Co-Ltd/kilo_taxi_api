using KiloTaxi.Model.DTO;

namespace KiloTaxi.DataAccess.Interface;

public interface IVehicleTypeRepository
{
    VehicleTypePagingDTO GetAllVehicleTypes(PageSortParam pageSortParam);
    VehicleTypeDTO AddVehicleType(VehicleTypeDTO vehicleTypeDTO);
    bool UpdateVehicleType(VehicleTypeDTO vehicleTypeDTO);
    VehicleTypeDTO GetVehicleTypeById(int id);
    bool DeleteVehicleType(int id);
}
