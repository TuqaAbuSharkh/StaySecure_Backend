using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StaySecure.DAL.Models
{
    
    public class ApplicationUserTranslations
    {
        public int Id { get; set; }

        public string FullName { get; set; }
        public string? City { get; set; }
        public string Language { get; set; } = "en";

        public string? ApplicationUserId { get; set; }
        [ForeignKey(nameof(ApplicationUserId))]
        public ApplicationUser? ApplicationUser { get; set; }

    }
}
