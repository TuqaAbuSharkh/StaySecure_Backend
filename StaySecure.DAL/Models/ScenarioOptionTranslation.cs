using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaySecure.DAL.Models
{
    public class ScenarioOptionTranslation
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public string Language { get; set; } = "en";

        public int ScenarioOptionId { get; set; }
        public ScenarioOption ScenarioOption { get; set; }
    }
}
