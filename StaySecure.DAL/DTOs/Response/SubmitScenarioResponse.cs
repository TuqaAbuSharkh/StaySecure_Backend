using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaySecure.DAL.DTOs.Response
{
    public class SubmitScenarioResponse : BaseRespose
    {
        public bool? IsCorrect { get; set; }

        public string? Feedback { get; set; }
    }
}
