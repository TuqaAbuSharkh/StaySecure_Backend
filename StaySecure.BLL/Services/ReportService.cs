using Microsoft.AspNetCore.Identity;
using StaySecure.BLL.Services.IServices;
using StaySecure.DAL.DTOs.Response.Reports;
using StaySecure.DAL.Models;
using StaySecure.DAL.Repositories.Interface;

namespace StaySecure.BLL.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReportService(
            IReportRepository reportRepository,
            UserManager<ApplicationUser> userManager)
        {
            _reportRepository = reportRepository;
            _userManager = userManager;
        }

        public async Task<List<TopicStatisticsResponse>>
            GetTopicStatisticsAsync()
        {
            return await _reportRepository
                .GetTopicStatisticsAsync();
        }

        public async Task<AwarenessReportResponse>
            GetAwarenessReportAsync()
        {
            var topicStatistics = await _reportRepository.GetTopicStatisticsAsync();

            var students =await _userManager.GetUsersInRoleAsync("Student");
            var ageGroupPerformance = await _reportRepository.GetAgeGroupPerformanceAsync();

            return new AwarenessReportResponse
            {
                TotalStudents = students.Count,

                OverallAwareness =await _reportRepository .GetOverallAwarenessAsync(),

                StrongestAgeGroup = await _reportRepository.GetStrongestAgeGroupAsync(),

                NeedsAttentionCount =topicStatistics.Count(x =>x.AveragePercentage < 50),

                GeneratedAt = DateTime.UtcNow,

                AgeGroupPerformance =ageGroupPerformance,

                TopicStatistics = topicStatistics,

                TopTopics = topicStatistics
                    .OrderByDescending(x =>
                        x.AveragePercentage)
                    .Take(5)
                    .Select(x =>
                        new TopicRankingResponse
                        {
                            Topic = x.Topic,
                            Percentage = x.AveragePercentage
                        })
                    .ToList(),

                WeakTopics = topicStatistics
                    .Where(x =>
                        x.AveragePercentage < 70)
                    .OrderBy(x =>
                        x.AveragePercentage)
                    .Take(5)
                    .Select(x =>
                        new TopicRankingResponse
                        {
                            Topic = x.Topic,
                            Percentage = x.AveragePercentage
                        })
                    .ToList()
            };
        }
    }
}