using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaySecure.DAL.Models
{
    public class BaseModel
    {
        public int Id { get; set; }

        public DateTime CreatedAt { set; get; } = DateTime.UtcNow;

        public string CreatedBy { get; set; }

        public DateTime UpdatedAt { set; get; } = DateTime.UtcNow;

        public string? UpdatedBy { get; set; }
        public Status? Status { get; set; }

        [ForeignKey("CreatedBy")]
        public ApplicationUser User { get; set; }

    }
}
