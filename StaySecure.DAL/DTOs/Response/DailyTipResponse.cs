using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaySecure.DAL.DTOs.Response
{
    public class DailyTipResponse
    {
        public string Tip { get; set; }

        public string Category { get; set; }

        public DateTime GeneratedDate { get; set; }
    }
}
