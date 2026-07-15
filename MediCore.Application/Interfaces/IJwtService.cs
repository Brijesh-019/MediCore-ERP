using MediCore.Shared.Models;

namespace MediCore.Application.Interfaces;

public interface IJwtService
{
    TokenResult GenerateTokens(
        long userId,
        string userName,
        string sessionId);
}