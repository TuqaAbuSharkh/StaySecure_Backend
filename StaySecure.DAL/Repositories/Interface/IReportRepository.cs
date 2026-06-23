using StaySecure.DAL.DTOs.Response.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaySecure.DAL.Repositories.Interface
{
    public interface IReportRepository
    {
        Task<List<TopicStatisticsResponse>>GetTopicStatisticsAsync();

        Task<int> GetTotalStudentsAsync();

        Task<double> GetOverallAwarenessAsync();
        Task<string> GetStrongestAgeGroupAsync();
        Task<List<AgeGroupPerformanceResponse>>GetAgeGroupPerformanceAsync();

    }
}
