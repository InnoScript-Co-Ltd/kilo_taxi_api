using System.ComponentModel.DataAnnotations;
using KiloTaxi.Common.Enums;

namespace KiloTaxi.Model.DTO;

public class ScheduleBookingDTO
{
    public int Id { get; set; }

    [Required]
    public string PickUpLocation { get; set; }

    [Required]
    public string DropOffLocation { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    public DateTime ScheduleTime { get; set; }

    [Required]
    public ScheduleStatus Status { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime CreatedDate { get; set; }

    [Required]
    public int CustomerId { get; set; }

    [Required]
    public int DriverId { get; set; }

    public IEnumerable<OrderDTO>? Orders { get; set; }
}
