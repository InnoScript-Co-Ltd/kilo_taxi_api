
using System.ComponentModel.DataAnnotations;
using KiloTaxi.Common.Enums;

namespace KiloTaxi.Model.DTO.Request;

public class SmsInfoDTO
{
    public int Id { get; set; }

    public string MobileNumber { get; set; }

    public string Name { get; set; }

    public string Title { get; set; }

    public string Message { get; set; }

    public SmsStatus Status { get; set; }

    public int AdminId { get; set; }
    public string? AdminName { get; set; }

    public int CustomerId { get; set; }

    public string? CustomerName { get; set; }

    public int DriverId { get; set; }

    public string? DriverName { get; set; }
}
