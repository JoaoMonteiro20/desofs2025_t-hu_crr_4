using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoImpact.DataModel.Models
{
    public class FootprintSummary
    {
        public Guid FootprintSummaryId { get; set; }

        public Guid UserId { get; set; }
        public DateOnly PeriodStart { get; set; }
        public DateOnly PeriodEnd { get; set; }
        public decimal TotalFootprint { get; set; }
        public DateTime GeneratedOn { get; set; }

        public User User { get; set; } = null!;
    }
}
