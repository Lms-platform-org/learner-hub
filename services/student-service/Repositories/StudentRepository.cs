//using LearningPlatform.StudentService.Data;
//using LearningPlatform.StudentService.Models;
//using Microsoft.EntityFrameworkCore;

//namespace LearningPlatform.StudentService.Repositories
//{
//    public class StudentRepository(StudentDbContext context) : IStudentRepository
//    {
//        public async Task<StudentProgress?> GetProgressAsync(string studentId, int courseId) =>
//            await context.ProgressRecords.FirstOrDefaultAsync(p => p.StudentId == studentId && p.CourseId == courseId);

//        public async Task AddProgressAsync(StudentProgress progress)
//        {
//            await context.ProgressRecords.AddAsync(progress);
//            await context.SaveChangesAsync();
//        }

//        public async Task UpdateProgressAsync(StudentProgress progress)
//        {
//            context.ProgressRecords.Update(progress);
//            await context.SaveChangesAsync();
//        }

//        public async Task<List<int>> GetTrendingCourseIdsAsync(int count) =>
//            await context.ProgressRecords
//                .GroupBy(p => p.CourseId)
//                .OrderByDescending(g => g.Count())
//                .Select(g => g.Key)
//                .Take(count)
//                .ToListAsync();

//        public async Task AddBookmarkAsync(Bookmark bookmark)
//        {
//            await context.Bookmarks.AddAsync(bookmark);
//            await context.SaveChangesAsync();
//        }

//        public async Task<List<int>> GetBookmarkCourseIdsAsync(string studentId) =>
//            await context.Bookmarks
//                .Where(b => b.StudentId == studentId)
//                .Select(b => b.CourseId)
//                .ToListAsync();

//        public async Task<List<int>> GetRecommendationsFromSimilarStudentsAsync(string studentId, List<int> studentCourseIds, int count) =>
//            await context.Bookmarks
//                .Where(b => studentCourseIds.Contains(b.CourseId) && b.StudentId != studentId)
//                .Select(b => b.StudentId)
//                .Distinct()
//                .Join(context.Bookmarks, sid => sid, b => b.StudentId, (sid, b) => b.CourseId)
//                .Where(cid => !studentCourseIds.Contains(cid))
//                .GroupBy(cid => cid)
//                .OrderByDescending(g => g.Count())
//                .Select(g => g.Key)
//                .Take(count)
//                .ToListAsync();

//        public async Task AddReviewAsync(Review review)
//        {
//            await context.Reviews.AddAsync(review);
//            await context.SaveChangesAsync();
//        }

//        public async Task<List<Review>> GetReviewsByCourseAsync(int courseId) =>
//            await context.Reviews.Where(r => r.CourseId == courseId).ToListAsync();
//    }
//}
