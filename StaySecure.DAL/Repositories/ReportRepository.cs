using Microsoft.EntityFrameworkCore;
using StaySecure.DAL.Data;
using StaySecure.DAL.DTOs.Response.Reports;
using StaySecure.DAL.Models;
using StaySecure.DAL.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaySecure.DAL.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly ApplicationDbContext _context;

        public ReportRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<TopicStatisticsResponse>> GetTopicStatisticsAsync()
        {
            var result = await _context.UserScenarios
                    .Join(
                        _context.Users,
                        us => us.UserId,
                        u => u.Id,
                        (us, u) => new { us, u })
                    .Join(
                        _context.Scenarios,
                        x => x.us.ScenarioId,
                        s => s.Id,
                        (x, s) => new { x.us, x.u, s })
                    .Join(
                        _context.ScenarioTranslations
                            .Where(t => t.Language == "en"),
                        x => x.s.Id,
                        t => t.ScenarioId,
                        (x, t) => new
                        {
                            Category =t.Category.Trim().ToLower(),
                            AgeGroup = x.u.AgeGroup,
                            IsCorrect = x.us.IsCorrect
                        })
                    .ToListAsync();

            return result
                .GroupBy(x => x.Category)
                .Select(g => new TopicStatisticsResponse
                {
                    Topic =CultureInfo.CurrentCulture.TextInfo.ToTitleCase(g.Key),
                    ChildPercentage =
                        CalculatePercentage(
                            g.Where(x =>
                                x.AgeGroup ==
                                AgeGroupEnum.Child)
                        ),

                    TeenPercentage =
                        CalculatePercentage(
                            g.Where(x =>
                                x.AgeGroup ==
                                AgeGroupEnum.Teen)
                        ),

                    AdultPercentage =
                        CalculatePercentage(
                            g.Where(x =>
                                x.AgeGroup ==
                                AgeGroupEnum.Adult)
                        ),

                    AveragePercentage =
                        CalculatePercentage(g)
                })
                .OrderByDescending(x =>
                    x.AveragePercentage)
                .ToList();
        }


        private static double CalculatePercentage(IEnumerable<dynamic> records)
        {
            var total = records.Count();

            if (total == 0)
                return 0;

            var correct =
                records.Count(x => x.IsCorrect);

            return Math.Round(
                (double)correct / total * 100,
                1);
        }

        public async Task<int> GetTotalStudentsAsync()
        {
            return await _context.Users.CountAsync();
        }

        public async Task<double> GetOverallAwarenessAsync()
        {
            var total =
                await _context.UserScenarios.CountAsync();

            if (total == 0)
                return 0;

            var correct =
                await _context.UserScenarios
                    .CountAsync(x => x.IsCorrect);

            return Math.Round(
                (double)correct / total * 100,
                1);
        }

        public async Task<string> GetStrongestAgeGroupAsync()
        {
            var result = await _context.UserScenarios
                .Join(
                    _context.Users,
                    us => us.UserId,
                    u => u.Id,
                    (us, u) => new
                    {
                        u.AgeGroup,
                        us.IsCorrect
                    })
                .ToListAsync();

            var strongest = result
                .GroupBy(x => x.AgeGroup)
                .Select(g => new
                {
                    AgeGroup = g.Key,
                    Percentage =
                        (double)g.Count(x => x.IsCorrect)
                        / g.Count() * 100
                })
                .OrderByDescending(x => x.Percentage)
                .FirstOrDefault();

            return strongest?.AgeGroup.ToString()
                   ?? "N/A";
        }

        public async Task<List<AgeGroupPerformanceResponse>>GetAgeGroupPerformanceAsync()
        {
            var result = await _context.UserScenarios
                .Join(
                    _context.Users,
                    us => us.UserId,
                    u => u.Id,
                    (us, u) => new
                    {
                        u.AgeGroup,
                        us.IsCorrect
                    })
                .ToListAsync();

            var percentages = result
                .GroupBy(x => x.AgeGroup)
                .Select(g => new AgeGroupPerformanceResponse
                {
                    AgeGroup = g.Key.ToString(),

                    Percentage = Math.Round(
                        (double)g.Count(x => x.IsCorrect)
                        / g.Count() * 100,
                        1)
                })
                .ToList();

            return new List<AgeGroupPerformanceResponse>
    {
        new()
        {
            AgeGroup = "Child",
            Percentage = percentages
                .FirstOrDefault(x => x.AgeGroup == "Child")
                ?.Percentage ?? 0
        },

        new()
        {
            AgeGroup = "Teen",
            Percentage = percentages
                .FirstOrDefault(x => x.AgeGroup == "Teen")
                ?.Percentage ?? 0
        },

        new()
        {
            AgeGroup = "Adult",
            Percentage = percentages
                .FirstOrDefault(x => x.AgeGroup == "Adult")
                ?.Percentage ?? 0
        }
    };
        }


    }
}
