using KiloTaxi.EntityFramework.EntityModel;

namespace KiloTaxi.Model.DTO;

public class OrderExtraDemandDTO
{
    public int Id { get; set; }
    
    public int OrderId { get; set; }
    
    public int ExtraDemandId { get; set; }
    
    public int Unit { get; set; }
    
    public ExtraDemandDTO ExtraDemandDto { get; set; }
    

}