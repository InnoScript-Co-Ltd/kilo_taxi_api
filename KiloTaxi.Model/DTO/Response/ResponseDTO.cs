using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace KiloTaxi.Model.DTO.Response;

public class ResponseDTO<T>
{
    public int StatusCode { get; set; }
    public string Message { get; set; }
    public DateTime TimeStamp { get; set; }

    public T Payload { get; set; }

    public List<T> PayloadList { get; set; }
}
