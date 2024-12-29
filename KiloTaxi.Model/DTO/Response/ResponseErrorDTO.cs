namespace KiloTaxi.Model.DTO.Response;

public class ResponseErrorDTO
{
    public int StatusCode { get; set; }
    public string Message { get; set; }
    public DateTime TimeStamp { get; set; }
}