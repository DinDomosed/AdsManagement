using AdsManagement.Domain.Models;

namespace AdsManagement.App.Exceptions
{
    public sealed class RoleNotFoundException : EntityNotFoundException
    {
        public RoleNotFoundException(Guid id) : base(nameof(Role), id) { }

    }
}
