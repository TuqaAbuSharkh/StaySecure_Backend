using StaySecure.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaySecure.DAL.DTOs.Request
{
    public class UpdateScenarioRequest
    {
        public int Id { get; set; }

        public LevelEnum Difficulty { get; set; }
        public int Score { get; set; }

        public List<ScenarioTranslationDto> Translations { get; set; }
        public List<ScenarioOptionDto> Options { get; set; }
    }
}
