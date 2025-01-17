using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiloTaxi.Model.DTO.Response;

public class ExtraDemandInfoDTO
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }
    
    public decimal Amount { get; set; }

    public DateTime CreateDate { get; set; }
}
