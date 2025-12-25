using AdsManagement.App.DTOs.Advertisement;

namespace AdsManagement.App.Interfaces.Service
{
    public interface IAdvertisementService
    {
        public Task<ResponceAdvertisementDto> GetAdvertisementAsync(Guid id, CancellationToken token = default);
        public Task<bool> IsOwnerAsync(Guid id, Guid userid, CancellationToken token = default);
    }
}
