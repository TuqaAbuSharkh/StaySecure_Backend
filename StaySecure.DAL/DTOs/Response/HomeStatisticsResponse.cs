using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaySecure.DAL.DTOs.Response
{
    public class HomeStatisticsResponse
    {
        public int Scenarios { get; set; }

        public int UsersTrained { get; set; }

        public int ThreatTypes { get; set; }

        public int DecisionsAnalyzed { get; set; }

        public int UserSatisfaction { get; set; }

        public string Availability { get; set; }
    }
}
