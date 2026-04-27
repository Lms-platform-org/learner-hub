using LearningPlatform.StudentService.DTOs;
using LearningPlatform.StudentService.Models;
using LearningPlatform.StudentService.Repositories;
using Microsoft.Extensions.Logging;

namespace LearningPlatform.StudentService.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IProfileRepository _repo;
        private readonly ILogger<ProfileService> _logger;

        public ProfileService(IProfileRepository repo, ILogger<ProfileService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<StudentProfile?> GetAsync(string studentId)
        {
            _logger.LogInformation("Fetching profile for user {UserId}", studentId);

            return await _repo.GetByStudentIdAsync(studentId);
        }

        public async Task SaveAsync(string studentId, ProfileDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FullName))
            {
                _logger.LogWarning("Full name is required for user {UserId}", studentId);
                throw new Exception("Full name is required");
            }

            if (dto.FullName.Length > 100)
            {
                _logger.LogWarning("Full name too long for user {UserId}", studentId);
                throw new Exception("Full name too long");
            }

            if (dto.Bio?.Length > 300)
            {
                _logger.LogWarning("Bio too long for user {UserId}", studentId);
                throw new Exception("Bio too long");
            }

            if (dto.Skills != null && dto.Skills.Count > 20)
            {
                _logger.LogWarning("Too many skills provided for user {UserId}", studentId);
                throw new Exception("Too many skills");
            }

            _logger.LogInformation("Updating profile for user {UserId}", studentId);

            var profile = new StudentProfile
            {
                StudentId = studentId,
                FullName = dto.FullName.Trim(),
                Bio = dto.Bio?.Trim(),
                Skills = dto.Skills?.Where(s => !string.IsNullOrWhiteSpace(s))
                                    .Select(s => s.Trim())
                                    .ToList() ?? new List<string>(),
                PreferredLevel = dto.PreferredLevel?.Trim() ?? "Beginner"
            };

            await _repo.UpsertAsync(profile);
        }
    }
}
