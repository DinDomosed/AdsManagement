using AdsManagement.App.DTOs.AdImage;

namespace AdsManagement.App.Interfaces.Service
{
    public interface IAdImageService
    {
        public Task<ResponseAdImageDto> GetImageAsync(Guid id, CancellationToken token = default);
        public Task<Guid> AddAdImageAsync(Guid adId, IFileData file, Guid requestUserId, CancellationToken token = default);
        public Task DeleteAdImageAsync(Guid id, Guid requestUserId, CancellationToken token = default);
        public Task<List<ResponseAdImageDto>> GetByAdIdAsync(Guid advertisementId, CancellationToken token = default);
       
    }
}
