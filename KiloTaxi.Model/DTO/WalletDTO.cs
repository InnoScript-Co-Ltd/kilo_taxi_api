using System.ComponentModel.DataAnnotations;
using KiloTaxi.Common.Enums;

namespace KiloTaxi.Model.DTO;

public class WalletDTO
{
    public int Id { get; set; }

    [Required]
    public string WalletName { get; set; }

    [Required]
    public DateTime CreatedDate { get; set; }

    [Required]
    public DateTime UpdateDate { get; set; }
}