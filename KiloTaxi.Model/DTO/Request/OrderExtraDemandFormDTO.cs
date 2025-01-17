namespace KiloTaxi.Model.DTO.Request;

public class OrderExtraDemandFormDTO
{
    public int Id { get; set; }
    
    public int OrderId { get; set; }
    
    public int ExtraDemandId { get; set; }
    
    public int Unit { get; set; }

}