using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace KiloTaxi.Model.DTO.Response;

public class ResponseDTO<T>
{
    public int StatusCode { get; set; }
    public string Message { get; set; }
    public DateTime TimeStamp { get; set; }
    
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] 
    public T Payload{ get; set; }
    
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    
    public IEnumerable<T> PayloadList { get; set; }
    
}