using LearningPlatformAuth.Data;

namespace LearningPlatformAuth.Services
{
    public interface ITokenService
    {
        Task<string> GenerateToken(ApplicationUser user);
    }
}
