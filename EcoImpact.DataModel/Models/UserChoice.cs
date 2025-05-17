using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoImpact.DataModel.Models
{
    public class UserChoice
    {
        public Guid UserChoiceId { get; set; }

        public Guid UserId { get; set; }
        public Guid HabitTypeId { get; set; }

        public decimal Quantity { get; set; }
        public string Unit { get; set; } = null!; // km, kWh, etc.
        public DateOnly Date { get; set; }
        public decimal Footprint { get; set; } // Quantity * Factor

        public User User { get; set; } = null!;
        public HabitType HabitType { get; set; } = null!;
    }
}
