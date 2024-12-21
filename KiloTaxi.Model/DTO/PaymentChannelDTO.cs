using System.ComponentModel.DataAnnotations;
using KiloTaxi.Common.Enums;

namespace KiloTaxi.Model.DTO;

public class PaymentChannelDTO
{
    public int Id { get; set; }

    [Required]
    public string ChannelName { get; set; }

    [Required]
    public string Description { get; set; }

    public PaymentType PaymentType { get; set; }

    public string Icon { get; set; }

    public string? Phone { get; set; }

    public string? UserName { get; set; }
}
