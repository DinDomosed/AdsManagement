using AdsManagement.App.Common;
using AdsManagement.Domain.Models;

namespace AdsManagement.App.Interfaces
{
    public interface ICommentStorage
    {
        public Task<Guid> AddAsync(Comment comment, CancellationToken token = default);
        public Task DeleteAsync(Guid commentId, CancellationToken token = default);
        public Task UpdateAsync(Comment comment, CancellationToken token = default);
        public Task<PagedResult<Comment>> GetByAdvertisementAsync(Guid advertisementId, int page, int pageSize, CancellationToken token = default);
    }
}
