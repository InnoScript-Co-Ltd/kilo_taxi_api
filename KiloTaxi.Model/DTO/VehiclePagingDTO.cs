using KiloTaxi.Model.DTO.Response;

namespace KiloTaxi.Model.DTO;

public class VehiclePagingDTO
{
    public PagingResult Paging { get; set; }
    public IEnumerable<VehicleInfoDTO> Vehicles { get; set; }
}