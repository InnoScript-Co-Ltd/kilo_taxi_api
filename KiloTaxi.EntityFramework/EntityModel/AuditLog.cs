using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiloTaxi.EntityFramework.EntityModel;

public class AuditLog
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [StringLength(100)]
    public string TableName { get; set; }
    
    [StringLength(100)]
    public string RecordId { get; set; }
    
    [StringLength(20)]
    public string Operation { get; set; }
    
    public string OldValues { get; set; }
    
    public string NewValues { get; set; }
    
    public string ChangedBy { get; set; }
    
    [DataType(DataType.DateTime)]
    public DateTime ChangedDate { get; set; }
    
}