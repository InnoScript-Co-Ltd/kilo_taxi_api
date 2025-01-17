using System.ComponentModel.DataAnnotations;

namespace KiloTaxi.Model.DTO.Request;

public class WalletFormDTO
{
    public int Id { get; set; }

    [Required]
    public string WalletName { get; set; }
    public DateTime CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }

}