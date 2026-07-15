namespace MediCore.Shared.Models;

public class TokenResult
{
    public string AccessToken { get; set; } = string.Empty;

    public DateTime AccessTokenExpiry { get; set; }

    public string RefreshToken { get; set; } = string.Empty;

    public DateTime RefreshTokenExpiry { get; set; }
}