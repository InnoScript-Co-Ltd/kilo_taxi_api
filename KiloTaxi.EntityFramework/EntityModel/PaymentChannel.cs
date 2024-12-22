using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiloTaxi.EntityFramework.EntityModel;

public class PaymentChannel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public string ChannelName { get; set; }

    [Required]
    public string Description { get; set; }

    public string PaymentType { get; set; }

    public string Icon { get; set; }

    public string? Phone { get; set; }

    public string? UserName { get; set; }
}
