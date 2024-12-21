using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiloTaxi.EntityFramework.EntityModel;

public class ScheduleBooking
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    public string PickUpLocation { get; set; }
    
    public string PickUpLat  { get; set; }
    
    public string PickUpLong  { get; set; }
    
    [Required]
    public string DestinationLocation { get; set; }
    
    public string DestinationLat  { get; set; }
    
    public string DestinationLong  { get; set; }
    
    [Required]
    [DataType(DataType.DateTime)]
    public DateTime ScheduleTime { get; set; }
    
    [Required]
    public string Status { get; set; }
    
    [DataType(DataType.DateTime)]
    public DateTime CreatedDate { get; set; }
    
    [ForeignKey("Customer")]
    public int CustomerId { get; set; }
    public virtual Customer Customer { get; set; }
    
    [ForeignKey("Driver")]
    public int DriverId { get; set; }
    public virtual Driver Driver { get; set; }
}