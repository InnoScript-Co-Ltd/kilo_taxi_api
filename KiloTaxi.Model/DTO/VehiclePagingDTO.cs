namespace KiloTaxi.Model.DTO;

public class VehiclePagingDTO
{
    public PagingResult Paging { get; set; }
    public IEnumerable<VehicleDTO> Vehicles { get; set; }
}