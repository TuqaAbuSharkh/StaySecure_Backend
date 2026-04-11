using StaySecure.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaySecure.DAL.DTOs.Response
{
    public class GetScenariosRequest
    {
        public string? Lang { get; set; } = "en";

        public string? Search { get; set; }

        public LevelEnum? Difficulty { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
