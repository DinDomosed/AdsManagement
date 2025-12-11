namespace AdsManagement.Domain.Models
{
    public class Role : BaseEntity
    {
        public string Name { get; private set; }
        public Role (string name, Guid? id = null) : base(id)
        {
            Name = name;
        }
        protected Role() : base() { }
        public override bool Equals(object? obj)
        {
            if (obj is not Role role)
                return false;

            return Id == role.Id;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
    }
}
