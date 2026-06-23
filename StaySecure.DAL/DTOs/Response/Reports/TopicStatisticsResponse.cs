using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaySecure.DAL.DTOs.Response.Reports
{
    public class TopicStatisticsResponse
    {
        public string Topic { get; set; }

        public double ChildPercentage { get; set; }

        public double TeenPercentage { get; set; }

        public double AdultPercentage { get; set; }

        public double AveragePercentage { get; set; }
    }
}
