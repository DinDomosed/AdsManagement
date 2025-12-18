using AdsManagement.Domain.Models;

namespace AdsManagement.App.Exceptions.NotFound
{
    public sealed class CommentNotFoundException : EntityNotFoundException
    {
        public CommentNotFoundException(Guid id) : base(nameof(Comment), id) { }
    }
}
