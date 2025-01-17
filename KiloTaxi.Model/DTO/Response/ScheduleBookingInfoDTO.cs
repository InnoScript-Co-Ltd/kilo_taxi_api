using System.ComponentModel.DataAnnotations;
using KiloTaxi.Common.Enums;

namespace KiloTaxi.Model.DTO.Response;

public class ScheduleBookingInfoDTO
{
    public int Id { get; set; }

    public string PickUpLocation { get; set; }
    
    public string PickUpLat  { get; set; }
    
    public string PickUpLong  { get; set; }
    
    public string DestinationLocation { get; set; }
    
    public string DestinationLat { get; set; }
    
    public string DestinationLong { get; set; }
    
    public DateTime ScheduleTime { get; set; }

    public ScheduleStatus Status { get; set; }

    public DateTime CreatedDate { get; set; }

    public int CustomerId { get; set; }

    public int DriverId { get; set; }

    public IEnumerable<OrderDTO>? Orders { get; set; }
}
