namespace KiloTaxi.Model.DTO;

public class TransactionLogDTO
{
    public int Id { get; set; }
    
    public int TransactionId { get; set; }
    
    public DateTime LogDate { get; set; }
    
    public string OperationType { get; set; }
    
    public string Details { get; set; }
    
    public string PerformedBy { get; set; }
}