using System.ComponentModel.DataAnnotations;

namespace KiloTaxi.Model.DTO.Response;

public class WalletInfoDTO
{
    public int Id { get; set; }

    public string WalletName { get; set; }
    
    public DateTime CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }
}