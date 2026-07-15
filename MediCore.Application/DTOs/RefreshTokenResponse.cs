namespace MediCore.Application.DTOs;

public class RefreshTokenResponse
{
    public string AccessToken { get; set; } = string.Empty;

    public DateTime AccessTokenExpiry { get; set; }

    public DateTime RefreshTokenExpiry { get; set; }
}