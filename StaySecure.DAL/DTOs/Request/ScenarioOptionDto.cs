using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaySecure.DAL.DTOs.Request
{
    public class ScenarioOptionDto
    {
        public bool IsCorrect { get; set; }
        public List<ScenarioOptionTranslationDto> Translations { get; set; }
    }
}
