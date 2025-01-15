using KiloTaxi.Model.DTO.Response;

namespace KiloTaxi.Model.DTO;

public class VehicleTypePagingDTO
{
    public PagingResult Paging { get; set; }
    public IEnumerable<VehicleTypeInfoDTO> VehicleTypes { get; set; }
}
