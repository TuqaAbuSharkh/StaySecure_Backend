using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaySecure.DAL.DTOs.Request
{
    public class SubmitScenarioRequest
    {
        public int ScenarioId { get; set; }

        public int OptionId { get; set; }
        public bool HintUsed { get; set; }
    }
}
