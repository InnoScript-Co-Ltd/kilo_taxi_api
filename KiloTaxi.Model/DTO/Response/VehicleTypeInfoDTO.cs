using System.ComponentModel.DataAnnotations;

namespace KiloTaxi.Model.DTO.Response;

public class VehicleTypeInfoDTO
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }
}
