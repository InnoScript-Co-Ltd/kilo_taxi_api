using System.ComponentModel.DataAnnotations.Schema;

namespace KiloTaxi.Model.DTO.Request;

public class TravelRateFormDTO
{
    public int Id { get; set; }
    
    public string Unit { get; set; }
    
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Rate { get; set; }
    
    public int CityId { get; set; }
    
    public string? CityName { get; set; }
    
    public int VehicleTypeId { get; set; }
    
    public string? VehicleTypeName { get; set; }
}