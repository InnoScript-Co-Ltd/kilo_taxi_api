using System.ComponentModel.DataAnnotations;
using KiloTaxi.Common.Enums;

namespace KiloTaxi.Model.DTO;

public class ScheduleBookingDTO
{
    public int Id { get; set; }
    
    [Required]
    public string PickUpAddress { get; set; }
    
    [Required]
    public string Destination { get; set; }
    
    [Required]
    [DataType(DataType.DateTime)]
    public DateTime ScheduleTime { get; set; }
    
    [Required]
    public ScheduleStatus Status { get; set; }
    
    [Required]
    public KiloType KiloType { get; set; }
    
    [Required]
    public int CustomerId { get; set; }
    
    [Required]
    public int DriverId { get; set; }
}