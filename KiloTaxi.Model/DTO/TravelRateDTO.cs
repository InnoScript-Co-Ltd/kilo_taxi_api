namespace KiloTaxi.Model.DTO;

public class TravelRateDTO
{
    public int Id { get; set; }
    
    public string Unit { get; set; }
    
    public string Rate { get; set; }
    
    public int CityId { get; set; }
    
    public int VehicleTypeId { get; set; }
}