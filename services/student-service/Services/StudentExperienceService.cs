using LearningPlatform.StudentService.Models;
using LearningPlatform.StudentService.Repositories;
using LearningPlatform.StudentService.DTOs;
using Microsoft.Extensions.Logging;

namespace LearningPlatform.StudentService.Services
{
    public class StudentExperienceService : IStudentExperienceService
    {
        private readonly IProgressRepository _progressRepo;
        private readonly ICourseIntegrationService _courseService;
        private readonly ILogger<StudentExperienceService> _logger;

        public StudentExperienceService(
            IProgressRepository progressRepo,
            ICourseIntegrationService courseService,
            ILogger<StudentExperienceService> logger)
        {
            _progressRepo = progressRepo;
            _courseService = courseService;
            _logger = logger;
        }

        public async Task<StudentProgress> MarkLessonCompleteAsync(string sid, int cid, int lid)
        {
            if (cid <= 0 || lid <= 0)
                throw new Exception("Invalid course or lesson id");

            var record = await GetOrCreateProgress(sid, cid);

            if (record.CompletedLessonIds.Contains(lid))
            {
                _logger.LogInformation("Lesson {LessonId} already completed", lid);
                return record;
            }

            _logger.LogInformation("Marking lesson {LessonId} complete", lid);

            record.CompletedLessonIds.Add(lid);
            record.XPScore += 10;

            var totalLessons = await _courseService.GetTotalLessonsForCourseAsync(cid);
            if (totalLessons <= 0)
                throw new Exception("Unable to fetch course lesson count");

            record.Percentage = Math.Min(
                (record.CompletedLessonIds.Count * 100) / totalLessons,
                100
            );

            AwardMilestones(record);

            record.LastAccessed = DateTime.UtcNow;

            await _progressRepo.UpdateAsync(record);

            // notify Sudhish's service of updated progress
            await _courseService.NotifyProgressAsync(sid, cid, record.Percentage);

            return record;
        }

        private void AwardMilestones(StudentProgress record)
        {
            var milestones = new Dictionary<int, string>
            {
                { 10,  "First Step" },
                { 50,  "Getting Started" },
                { 100, "Century" },
                { 250, "Quarter Master" },
                { 500, "Half Way Hero" },
                { 1000,"XP Legend" }
            };

            foreach (var (xpThreshold, badge) in milestones)
            {
                if (record.XPScore >= xpThreshold && !record.EarnedMilestones.Contains(badge))
                {
                    record.EarnedMilestones.Add(badge);
                    _logger.LogInformation("Milestone unlocked: {Badge} for course {CourseId}", badge, record.CourseId);
                }
            }

            if (record.Percentage == 100 && !record.EarnedMilestones.Contains("Course Complete"))
            {
                record.EarnedMilestones.Add("Course Complete");
                _logger.LogInformation("Course Complete milestone awarded for course {CourseId}", record.CourseId);
            }
        }

        public async Task SetResumePointAsync(string sid, int cid, int lid, int seconds)
        {
            if (cid <= 0 || lid <= 0 || seconds < 0)
                throw new Exception("Invalid resume data");

            var record = await GetOrCreateProgress(sid, cid);

            _logger.LogInformation("Setting resume point for course {CourseId}", cid);

            record.LastLessonId = lid;
            record.LastVideoTimestampSeconds = seconds;
            record.LastAccessed = DateTime.UtcNow;

            await _progressRepo.UpdateAsync(record);
        }

        public async Task<StudentProgress?> GetProgressAsync(string sid, int cid)
        {
            if (cid <= 0)
                throw new Exception("Invalid course id");

            _logger.LogInformation("Fetching progress for course {CourseId}", cid);

            return await _progressRepo.GetProgressAsync(sid, cid);
        }

        private async Task<StudentProgress> GetOrCreateProgress(string sid, int cid)
        {
            var record = await _progressRepo.GetProgressAsync(sid, cid);

            if (record != null)
                return record;

            _logger.LogInformation("Creating new progress record for course {CourseId}", cid);

            record = new StudentProgress
            {
                StudentId = sid,
                CourseId = cid,
                CompletedLessonIds = new List<int>(),
                EarnedMilestones = new List<string>()
            };

            await _progressRepo.AddAsync(record);

            return record;
        }
    }
}