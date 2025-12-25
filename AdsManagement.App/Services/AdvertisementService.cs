using AdsManagement.App.DTOs.Advertisement;
using AdsManagement.App.Interfaces.Service;
using AdsManagement.App.Interfaces.Storage;
using AutoMapper;

namespace AdsManagement.App.Services
{
    public class AdvertisementService : IAdvertisementService
    {
        private readonly IAdvertisementStorage _storage;
        private readonly IMapper _mapper;
        public AdvertisementService(IAdvertisementStorage storage, IMapper mapper)
        {
            _storage = storage;
            _mapper = mapper;
        }
        public async Task<ResponceAdvertisementDto> GetAdvertisementAsync(Guid id, CancellationToken token = default)
        {
            var advertisementDb = await _storage.GetAsync(id, token);
            return _mapper.Map<ResponceAdvertisementDto>(advertisementDb);
        }
        public async Task<bool> IsOwnerAsync(Guid id, Guid userid, CancellationToken token = default)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("The advertisement ID cannot be empty.");
            if (userid == Guid.Empty)
                throw new ArgumentException("The user's ID cannot be empty");

            var dbAd = await _storage.GetAsync(id, token);
            
            if (dbAd.UserId == userid)
                return true;
            return false;
        }
    }
}
