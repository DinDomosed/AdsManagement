using AdsManagement.Domain.Models;

namespace AdsManagement.App.Exceptions
{
    public sealed class UserNotFoundException : EntityNotFoundException
    {
        public UserNotFoundException(Guid id) : base(nameof(User), id) { }
    }
}
