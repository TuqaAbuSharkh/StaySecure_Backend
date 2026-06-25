using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaySecure.DAL.DTOs.Response
{
    public class AiChallengeDto
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string Category { get; set; }

        public string Hint { get; set; }

        public List<AiChallengeOptionDto> Options { get; set; }
    }
}
