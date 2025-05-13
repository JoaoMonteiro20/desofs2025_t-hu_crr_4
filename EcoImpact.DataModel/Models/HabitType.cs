using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoImpact.DataModel.Models
{
    public class HabitType
    {
        public Guid HabitTypeId { get; set; }
        public string Name { get; set; } = null!;
        public decimal Factor { get; set; } // kg CO₂e por unidade

        public ICollection<UserChoice> UserChoices { get; set; } = new List<UserChoice>();
    }
}
