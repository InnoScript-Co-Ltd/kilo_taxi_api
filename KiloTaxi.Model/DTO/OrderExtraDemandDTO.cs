using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Model.DTO.Response;

namespace KiloTaxi.Model.DTO;

public class OrderExtraDemandDTO
{
    public int Id { get; set; }
    
    public int OrderId { get; set; }
    
    public int ExtraDemandId { get; set; }
    
    public int Unit { get; set; }
    
    public ExtraDemandInfoDTO ExtraDemandDto { get; set; }
    

}