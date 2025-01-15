using System.ComponentModel.DataAnnotations;

namespace KiloTaxi.Model.DTO.Request;

public class VehicleTypeFormDTO
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    public string Description { get; set; }
}
