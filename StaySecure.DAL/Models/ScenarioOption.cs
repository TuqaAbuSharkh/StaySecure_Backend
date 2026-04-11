using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaySecure.DAL.Models
{
    public class ScenarioOption
    {
        public int Id { get; set; }

        public ICollection<ScenarioOptionTranslation> Translations { get; set; }

        public bool IsCorrect { get; set; }

        public int ScenarioId { get; set; }
        public Scenario Scenario { get; set; }
    }

}
