using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaySecure.DAL.DTOs.Response
{
    public class ChallengeCacheDto
    {
        public int CorrectAnswerId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Category { get; set; }
    }
}
