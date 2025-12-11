using AdsManagement.App.Common;
using AdsManagement.Domain.Models;

namespace AdsManagement.App.Interfaces
{
    public interface IRoleStorage : IStorage<Role>
    {
        public  Task<PagedResult<Role>> GetAllAsync(int page , int PageSize , CancellationToken token = default);
    }
}
