using LearningPlatform.StudentService.Data;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.StudentService
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StudentDbContext>();
            await context.Database.MigrateAsync();
        }
    }
}
