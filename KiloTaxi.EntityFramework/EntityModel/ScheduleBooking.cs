using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiloTaxi.EntityFramework.EntityModel;

public class ScheduleBooking
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    public string PickUpAddress{ get; set; }
    
    [Required]
    public string Destination { get; set; }
    
    [Required]
    [DataType(DataType.DateTime)]
    public DateTime ScheduleTime { get; set; }
    
    [Required]
    public string Status { get; set; }
    
    [Required]
    public string KiloType {get; set;}
    
    [ForeignKey("Customer")]
    public int CustomerId { get; set; }
    public virtual Customer Customer { get; set; }
    
    [ForeignKey("Driver")]
    public int DriverId { get; set; }
    public virtual Driver Driver { get; set; }
}