using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiloTaxi.EntityFramework.EntityModel;

public class Notification
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    public string Message { get; set; }
    
    public string? ImageUrl { get; set; }
    
    [Required]
    public string Title { get; set; }
    
    [Required]
    public string Status {get; set;}
    
    [Required]
    public string NotificationType { get; set; }
    
    [ForeignKey("Admin")]
    public int AdminId { get; set; }
    public virtual Admin Admin { get; set; }
    
    [ForeignKey("Customer")]
    public int CustomerId { get; set; }
    public virtual Customer Customer { get; set; }
    
    [ForeignKey("Driver")]
    public int DriverId { get; set; }
    public virtual Driver Driver { get; set; }
    
}