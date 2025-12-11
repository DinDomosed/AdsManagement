namespace AdsManagement.Domain.Models
{
    public abstract class BaseEntity
    {
        public Guid Id { get; private set; }
        public BaseEntity(Guid? id)
        {
            Id = id ?? Guid.NewGuid();
        }
        protected BaseEntity() { }
    }
}
