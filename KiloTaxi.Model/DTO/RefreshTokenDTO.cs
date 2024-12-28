namespace KiloTaxi.Model.DTO;

public class RefreshTokenDTO
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    
    public string? Otp { get; set; }
}