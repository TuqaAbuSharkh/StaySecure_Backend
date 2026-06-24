using Microsoft.AspNetCore.Identity;
using StaySecure.BLL.Services.IServices;
using StaySecure.DAL.DTOs.Response;
using StaySecure.DAL.DTOs.Response.Reports;
using StaySecure.DAL.Models;
using StaySecure.DAL.Repositories.Interface;

namespace StaySecure.BLL.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IScenarioRepository _scenarioRepository;
        private readonly IAiService _aiService;

        public ReportService( IReportRepository reportRepository,
            UserManager<ApplicationUser> userManager,
            IScenarioRepository scenarioRepository,
            IAiService aiService)
        {
            _reportRepository = reportRepository;
            _userManager = userManager;
            _scenarioRepository = scenarioRepository;
            _aiService = aiService;
        }
        public async Task<List<TopicStatisticsResponse>>
            GetTopicStatisticsAsync()
        {
            return await _reportRepository
                .GetTopicStatisticsAsync();
        }

        public async Task<AwarenessReportResponse>  GetAwarenessReportAsync()
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


        public async Task<DailyTipResponse?>GetDailyTipAsync(string userId)
        {
            var todayTip =
                await _reportRepository
                    .GetTodayTipAsync(userId);

            if (todayTip != null)
            {
                return new DailyTipResponse
                {
                    Tip = todayTip.Tip,
                    Category = todayTip.Category,
                    GeneratedDate =
                        todayTip.GeneratedDate
                };
            }

            var weakCategories =
                await _scenarioRepository
                    .GetTopWeakCategoriesAsync(userId);

            var category =
                weakCategories.FirstOrDefault()
                ?? "General Cybersecurity";

            var generatedTip =
                await _aiService
                    .GenerateDailyTipAsync(category);

            if (string.IsNullOrWhiteSpace(generatedTip))
                return null;

            var newTip = new UserDailyTip
            {
                UserId = userId,
                Category = category,
                Tip = generatedTip,
                GeneratedDate = DateTime.UtcNow
            };

            await _reportRepository
                .AddDailyTipAsync(newTip);

            return new DailyTipResponse
            {
                Tip = newTip.Tip,
                Category = newTip.Category,
                GeneratedDate =
                    newTip.GeneratedDate
            };
        }

    }
}