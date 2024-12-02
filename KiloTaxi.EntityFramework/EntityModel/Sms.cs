using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiloTaxi.EntityFramework.EntityModel;

public class Sms
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
    
    [ForeignKey("Admin")]
    public int AdminId { get; set; }
    public Admin Admin { get; set; }
    
    [ForeignKey("Customer")]
    public int CustomerId { get; set; }
    public Customer Customer { get; set; }
    
    [ForeignKey("Driver")]
    public int DriverId { get; set; }
    public Driver Driver { get; set; }
}