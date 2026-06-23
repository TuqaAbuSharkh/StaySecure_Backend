using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaySecure.DAL.DTOs.Response.Reports
{
    public class AwarenessReportResponse
    {
        public int TotalStudents { get; set; }

        public double OverallAwareness { get; set; }

        public string StrongestAgeGroup { get; set; }

        public int NeedsAttentionCount { get; set; }

        public DateTime GeneratedAt { get; set; }

        public List<TopicStatisticsResponse> TopicStatistics { get; set; }

        public List<TopicRankingResponse> TopTopics { get; set; }

        public List<TopicRankingResponse> WeakTopics { get; set; }

        public List<AgeGroupPerformanceResponse> AgeGroupPerformance { get; set; }
    }
}
