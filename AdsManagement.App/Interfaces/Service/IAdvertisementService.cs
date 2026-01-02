using AdsManagement.App.Common;
using AdsManagement.App.DTOs.Advertisement;

namespace AdsManagement.App.Interfaces.Service
{
    public interface IAdvertisementService
    {
        public Task<ResponseAdvertisementDto> GetAdvertisementAsync(Guid id, CancellationToken token = default);
        public Task<Guid> AddAdvertisementAsync(CreateAdvertisementDto advertisementDto, IFileData? file = null, CancellationToken token = default);
        public Task DeleteAdvertisementAsync(Guid id, Guid reqestUserId, CancellationToken token = default);
        public Task UpdateAdvertisementAsync(UpdateAdvertisementDto advertisementDto, Guid reqestUserId, CancellationToken token = default);
        public Task<PagedResult<ResponseAdvertisementDto>> GetFilterAdsAsync(AdFilterDto filter, CancellationToken token = default);
        public Task<PagedResult<ResponseAdvertisementDto>> GetByUserAdsAsync(Guid userId, UserAdvertisementFilterDto filter, CancellationToken token = default);
        public Task RecalculateRatingAsync(Guid id, CancellationToken token = default); 
    }
}
