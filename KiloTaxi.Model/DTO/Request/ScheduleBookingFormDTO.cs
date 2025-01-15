using System.ComponentModel.DataAnnotations;
using KiloTaxi.Common.Enums;

namespace KiloTaxi.Model.DTO.Request;

public class ScheduleBookingFormDTO
{
    public int Id { get; set; }

    [Required]
    public string PickUpLocation { get; set; }
    
    public string PickUpLat  { get; set; }
    
    public string PickUpLong  { get; set; }
    
    [Required]
    public string DestinationLocation { get; set; }
    
    public string DestinationLat { get; set; }
    
    public string DestinationLong { get; set; }
    

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
