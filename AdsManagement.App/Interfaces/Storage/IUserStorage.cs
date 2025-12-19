using AdsManagement.App.Common;
using AdsManagement.App.DTOs.User;
using AdsManagement.Domain.Models;

namespace AdsManagement.App.Interfaces.Storage
{
    public interface IUserStorage: IStorage<User>
    {
        public Task<PagedResult<User>> GetFilterUserAsync(UserFilterDto userFilterDto, CancellationToken token = default);
    }
}
