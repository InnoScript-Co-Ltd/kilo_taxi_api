using KiloTaxi.Common.Enums;

namespace KiloTaxi.Model.DTO.Response;

public class CustomerInfoDTO
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    
    public string? Profile { get; set; }

    public string? MobilePrefix { get; set; }
    public string? Email { get; set; }
    
    
    public string Phone { get; set; }

    public string Role {get;set;}
    
    public string? Address { get; set; }

    public string? City { get; set; }

    public string? Township { get; set; }

    public GenderType? Gender { get; set; }
    
    public CustomerStatus Status { get; set; }

    public KycStatus KycStatus { get; set; }
}