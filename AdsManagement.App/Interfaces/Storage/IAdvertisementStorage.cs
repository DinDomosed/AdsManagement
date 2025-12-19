using AdsManagement.App.Common;
using AdsManagement.App.DTOs.Advertisement;
using AdsManagement.Domain.Models;

namespace AdsManagement.App.Interfaces.Storage
{
    public interface IAdvertisementStorage : IStorage<Advertisement>
    {
        public Task<PagedResult<Advertisement>> GetFilterAdsAsync(AdFilterDto adFilterDto, CancellationToken token = default);
        public Task<int> GetUserAdsCountActive(Guid userId, CancellationToken token = default);
        public Task<int> GetUserAdsCountAll(Guid userId, CancellationToken token = default);
        public Task<PagedResult<Advertisement>> GetUserAdsAsync(Guid userId, UserAdvertisementFilterDto filter, CancellationToken token = default);
    }
}
