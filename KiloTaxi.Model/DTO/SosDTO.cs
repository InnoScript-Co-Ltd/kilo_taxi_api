using System.ComponentModel.DataAnnotations;
using KiloTaxi.Common.Enums;

namespace KiloTaxi.Model.DTO;

public class SosDTO
{
    public int Id { get; set; }
    
    [Required]    
    public string Address { get; set; }
    
    [Required]
    public GeneralStatus Status  { get; set; }
    
    [Required]
    public int ReferenceId { get; set; }
    
    [Required]
    public WalletType WalletType { get; set; }
    
    public int ReasonId { get; set; }
    public string? ReasonName { get; set; }
}