using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;

namespace KiloTaxi.DataAccess.Interface;

public interface IVehicleTypeRepository
{
    ResponseDTO<VehicleTypePagingDTO> GetAllVehicleTypes(PageSortParam pageSortParam);
    VehicleTypeInfoDTO AddVehicleType(VehicleTypeFormDTO vehicleTypeFormDTO);
    bool UpdateVehicleType(VehicleTypeFormDTO vehicleTypeFormDTO);
    VehicleTypeInfoDTO GetVehicleTypeById(int id);
    bool DeleteVehicleType(int id);
}
