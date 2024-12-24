using System.ComponentModel.DataAnnotations;

namespace KiloTaxi.Model.DTO;

public class OderRouteDTO
{
    public int Id { get; set; }
    
    public string Lat { get; set; }
    
    public string Long { get; set; }
    
    [DataType(DataType.DateTime)]
    public DateTime CreateDate { get; set; }
    
    public int OrderId { get; set; }

}