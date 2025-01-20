using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiloTaxi.Model.DTO.Response;

public class OrderExtendInfoDTO
{
    public int Id { get; set; }
    
    public string DestinationLocation {get;set;}
    
    public string DestinationLat { get; set; }
    
    public string DestinationLong { get; set; }
    
    public DateTime CreateDate { get; set; }
    
    public int OrderId { get; set; }
}