using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoImpact.DataModel.Dtos
{
    public class UserChoiceDto
    {
        public Guid UserId { get; set; }
        public Guid HabitTypeId { get; set; }
        public decimal Quantity { get; set; }
        public DateOnly Date { get; set; }
    }
}
