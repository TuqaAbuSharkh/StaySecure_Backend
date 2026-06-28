using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaySecure.DAL.Models
{
    public class UserDailyTip
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public string Category { get; set; }

        public string Tip { get; set; }
        public string Language { get; set; }

        public DateTime GeneratedDate { get; set; }
    }
}
