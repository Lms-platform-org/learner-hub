using LearningPlatform.StudentService.Data;
using LearningPlatform.StudentService.Models;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.StudentService.Repositories
{
    public class ProfileRepository(StudentDbContext context) : IProfileRepository
    {
        public async Task<StudentProfile?> GetByStudentIdAsync(string sid) =>
            await context.StudentProfiles.FirstOrDefaultAsync(p => p.StudentId == sid);
        public async Task UpsertAsync(StudentProfile p)
        {
            var existing = await GetByStudentIdAsync(p.StudentId);
            if (existing == null)
                context.StudentProfiles.Add(p);
            else
            {
                existing.FullName = p.FullName;
                existing.Bio = p.Bio;
                existing.Skills = p.Skills;
                existing.PreferredLevel = p.PreferredLevel;
            }
            await context.SaveChangesAsync();
        }
    }
}
