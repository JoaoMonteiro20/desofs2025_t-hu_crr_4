using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EcoImpact.DataModel.Models
{
    public class User
    {
        public Guid UserId { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [JsonPropertyName("role")]
        public UserRole Role { get; set; } = UserRole.User;
        public ICollection<UserChoice> UserChoices { get; set; } = new List<UserChoice>();
        public ICollection<FootprintSummary> Summaries { get; set; } = new List<FootprintSummary>();
        public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

        public int FailedLoginAttempts { get; set; } = 0;
        public DateTime? LockoutEnd { get; set; } = null;
    }
}
