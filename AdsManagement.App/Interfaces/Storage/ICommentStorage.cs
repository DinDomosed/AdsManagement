using AdsManagement.App.Common;
using AdsManagement.Domain.Models;

namespace AdsManagement.App.Interfaces.Storage
{
    public interface ICommentStorage : IStorage<Comment>
    {
        public Task<PagedResult<Comment>> GetByAdvertisementAsync(Guid advertisementId, int page, int pageSize, CancellationToken token = default);
    }
}
