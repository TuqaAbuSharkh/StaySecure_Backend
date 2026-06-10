using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaySecure.DAL.DTOs.Response
{
    public class ScenarioPlayResponse
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Category { get; set; }

        public int Score { get; set; }

        public string? Hint { get; set; }

        public int HintPenalty { get; set; }
        public List<OptionResponse> Options { get; set; }
    }

}
