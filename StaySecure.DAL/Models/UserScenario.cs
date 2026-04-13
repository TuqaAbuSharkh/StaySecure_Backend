using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaySecure.DAL.Models
{
    public class UserScenario
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int ScenarioId { get; set; }

        public bool IsCorrect { get; set; }

        public DateTime CompletedAt { get; set; }
    }

}
