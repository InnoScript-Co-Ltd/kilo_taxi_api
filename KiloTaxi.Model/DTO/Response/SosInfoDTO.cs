using System.ComponentModel.DataAnnotations;
using KiloTaxi.Common.Enums;

namespace KiloTaxi.Model.DTO.Response;

public class SosInfoDTO
{
    public int Id { get; set; }
    
    public string Address { get; set; }
    
    public GeneralStatus Status  { get; set; }
    
    public int ReferenceId { get; set; }
    
    public UserType UserType { get; set; }
    
    public int ReasonId { get; set; }
    public string? ReasonName { get; set; }
}