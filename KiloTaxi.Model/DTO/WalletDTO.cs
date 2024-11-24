using System.ComponentModel.DataAnnotations;

namespace KiloTaxi.Model.DTO;

public class WalletDTO
{
    public int Id { get; set; }

    [Required]
    public string WalletName { get; set; }
    public DateTime CreatedDate { get; set; }

    public DateTime? UpdateDate { get; set; }

}