using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaySecure.DAL.Models
{
    public class Scenario
    {
        public int Id { get; set; }

        public LevelEnum Level { get; set; }
        public AgeGroupEnum AgeGroup { get; set; }

        public int Score { get; set; }

        public string? Hint { get; set; }

        public int HintPenalty { get; set; } = 0;

        public bool IsAiGenerated { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<ScenarioOption> Options { get; set; }
        public ICollection<ScenarioTranslation> Translations { get; set; }
    }

}
