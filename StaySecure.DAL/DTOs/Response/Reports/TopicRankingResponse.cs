using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaySecure.DAL.DTOs.Response.Reports
{
    public class TopicRankingResponse
    {
        public string Topic { get; set; }

        public double Percentage { get; set; }
    }
}
