using AdsManagement.Domain.Models;

namespace AdsManagement.App.Interfaces.Storage
{
    public interface IAdImageStorage
    {
        public Task<Guid> AddAsync(AdvertisementImage image, CancellationToken token = default);
        public Task DeleteAsync(Guid id, CancellationToken token = default);
        public Task<AdvertisementImage> GetAsync(Guid id, CancellationToken token = default); 
        public Task<List<AdvertisementImage>> GetByAdIdAsync(Guid advertisementId, CancellationToken token = default);
    }
}
