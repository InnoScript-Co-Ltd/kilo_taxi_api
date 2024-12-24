using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiloTaxi.Model.DTO;

public class OrderExtendDTO
{
    public int Id { get; set; }
    
    public string DestinationLocation {get;set;}
    
    public string DestinationLat { get; set; }
    
    public string DestinationLong { get; set; }
    
    [DataType(DataType.DateTime)]
    public DateTime CreateDate { get; set; }
    
    public int OrderId { get; set; }
}