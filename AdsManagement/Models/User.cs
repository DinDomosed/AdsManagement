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
        public Guid RoleId { get; private set; }
        public virtual Role Role { get; private set; }
        public User(string name,Guid roleId, Guid? id = null) : base(id)
        {
            Name = name;
            RoleId = roleId;
        }
        protected User() : base() { }
        public virtual ICollection<Advertisement> Advertisements { get; private set; } = new List<Advertisement>();

        public void UpdateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name), "The name cannot be empty");
            if (name.Length > 100)
                throw new ArgumentOutOfRangeException(nameof(name), "The name is too long");

            Name = name.Trim();
        }
        public void UpdateRole(Role role)
        {
            Role = role ?? throw new ArgumentNullException(nameof(role));
        }

        public override bool Equals(object? obj)
        {
            if (obj is not User user)
                return false;

            return Id == user.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
    }
}
