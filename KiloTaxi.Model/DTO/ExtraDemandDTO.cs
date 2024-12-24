using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiloTaxi.Model.DTO;

public class ExtraDemandDTO
{
    public int Id { get; set; }
    
    public string Title {get;set;}
    
    public string Description { get; set; }
    
    public int Unit { get; set; }
    
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Amount { get; set; }
    
    [DataType(DataType.DateTime)]
    public DateTime CreateDate { get; set; }
}