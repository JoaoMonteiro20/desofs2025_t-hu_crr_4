using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoImpact.DataModel.Models
{
    public class User
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }

        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;

        public Role Role { get; set; } = null!;
        public ICollection<UserChoice> UserChoices { get; set; } = new List<UserChoice>();
        public ICollection<FootprintSummary> Summaries { get; set; } = new List<FootprintSummary>();
        public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
    }
}
