using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiloTaxi.EntityFramework.EntityModel;

public class TransactionLog
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int TransactionId { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime LogDate { get; set; }

    [StringLength(50)]
    public string OperationType { get; set; }

    public string Details { get; set; }

    [StringLength(100)]
    public string PerformedBy { get; set; }
}
