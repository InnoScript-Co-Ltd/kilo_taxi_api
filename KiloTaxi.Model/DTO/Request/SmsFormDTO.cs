
using System.ComponentModel.DataAnnotations;
using KiloTaxi.Common.Enums;

namespace KiloTaxi.Model.DTO.Request;

public class SmsFormDTO
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
    public SmsStatus Status { get; set; }

    public int AdminId { get; set; }
    public string? AdminName { get; set; }

    public int CustomerId { get; set; }

    public string? CustomerName { get; set; }

    public int DriverId { get; set; }

    public string? DriverName { get; set; }
}
