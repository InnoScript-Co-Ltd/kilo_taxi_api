using System.ComponentModel.DataAnnotations;

namespace KiloTaxi.Model.DTO;

public class AuditLogDTO
{
    public int Id { get; set; }
    
    public string TableName { get; set; }
    
    public string RecordId { get; set; }
    
    public string Operation { get; set; }
    
    public string OldValues { get; set; }
    
    public string NewValues { get; set; }
    
    public string ChangedBy { get; set; }
    
    public DateTime ChangedDate { get; set; }
}