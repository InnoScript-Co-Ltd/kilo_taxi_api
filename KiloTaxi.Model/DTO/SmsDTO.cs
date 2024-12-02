namespace KiloTaxi.Model.DTO;
using System.ComponentModel.DataAnnotations;

public class SmsDTO
{
    public int Id { get; set; }
    
    [Required]
    public string MobileNumber { get; set; }
    
    [Required]
    public string Name { get; set; }
    
    [Required]
    public string Title { get; set; }
    
    [Required]
    public string Message { get; set; }
    
    [Required]
    public string Status  { get; set; }
    
    public int? AdminId { get; set; }
    
    public int? CustomerId { get; set; }
    
    public int? DriverId { get; set; }
}