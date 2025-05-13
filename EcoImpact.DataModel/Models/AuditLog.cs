using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoImpact.DataModel.Models
{
    public class AuditLog
    {
        public Guid LogId { get; set; }

        public Guid UserId { get; set; }
        public string ActionType { get; set; } = null!;
        public string TargetTable { get; set; } = null!;
        public Guid TargetId { get; set; }
        public DateTime Timestamp { get; set; }

        public User User { get; set; } = null!;
    }
