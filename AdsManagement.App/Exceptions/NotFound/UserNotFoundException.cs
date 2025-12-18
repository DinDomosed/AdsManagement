using AdsManagement.Domain.Models;

namespace AdsManagement.App.Exceptions.NotFound
{
    public sealed class UserNotFoundException : EntityNotFoundException
    {
        public UserNotFoundException(Guid id) : base(nameof(User), id) { }
    }
}
