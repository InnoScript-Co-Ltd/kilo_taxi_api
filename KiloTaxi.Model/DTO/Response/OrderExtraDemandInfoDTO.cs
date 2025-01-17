namespace KiloTaxi.Model.DTO.Response;

public class OrderExtraDemandInfoDTO
{
    public int Id { get; set; }
    
    public int OrderId { get; set; }
    
    public int ExtraDemandId { get; set; }
    
    public int Unit { get; set; }

    public  ExtraDemandInfoDTO ExtraDemandInfoDto { get; set; }
}