using System.ComponentModel.DataAnnotations.Schema;

namespace KiloTaxi.Model.DTO.Response;

public class TravelRateInfoDTO
{
    public int Id { get; set; }
    
    public string Unit { get; set; }
    
    public decimal Rate { get; set; }
    
    public int CityId { get; set; }
    
    public string? CityName { get; set; }
    
    public int VehicleTypeId { get; set; }
    
    public string? VehicleTypeName { get; set; }
}