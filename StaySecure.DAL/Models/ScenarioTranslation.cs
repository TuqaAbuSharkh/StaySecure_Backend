using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaySecure.DAL.Models
{
    public  class ScenarioTranslation
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public string Category { get; set; }

        public string Language { get; set; } = "en";

        public int ScenarioId { get; set; }
        public Scenario Scenario { get; set; }
    }
}
