using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdsManagement.Domain.Models
{
    public class User : BaseEntity
    {
        public string Name { get; private set; }
        public virtual Role Role { get; private set; }
        public User(string name, Role role, Guid? id = null) : base(id)
        {
            Name = name;
            Role = role;
        }
        public virtual ICollection<Advertisement> Advertisements { get; private set; } = new List<Advertisement>();
    }
}
