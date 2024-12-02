using System.ComponentModel.DataAnnotations;

namespace KiloTaxi.Model.DTO;

public class NotificationDTO
{
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
    
    public int AdminId { get; set; }
    
    public int CustomerId { get; set; }
    
    public int DriverId { get; set; }
}