using System.ComponentModel.DataAnnotations;

namespace KiloTaxi.Model.DTO;

public class KiloAmountDTO
{
    public int Id { get; set; }

    [Required]
    public int Kilo { get; set; }

    [Required]
    public decimal Amount { get; set; }
}
