using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoImpact.DataModel.Models
{
    public class Role
    {
        public Guid RoleId { get; set; }
        public string Name { get; set; } = null!; // admin | moderator | normalUser

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
