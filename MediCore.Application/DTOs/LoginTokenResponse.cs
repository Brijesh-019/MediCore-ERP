namespace MediCore.Application.DTOs;

public class LoginTokenResponse
{
    public LoginResponse User { get; set; } = new();
    
    public string AccessToken { get; set; } = string.Empty;

    public DateTime AccessTokenExpiry { get; set; }

    public DateTime RefreshTokenExpiry { get; set; }
}