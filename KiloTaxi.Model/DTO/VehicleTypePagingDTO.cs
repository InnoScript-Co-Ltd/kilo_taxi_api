namespace KiloTaxi.Model.DTO;

public class VehicleTypePagingDTO
{
    public PagingResult Paging { get; set; }
    public IEnumerable<VehicleTypeDTO> VehicleTypes { get; set; }
}
