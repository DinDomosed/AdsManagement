using AdsManagement.App.Common;
using AdsManagement.App.DTOs.Advertisement;
using AdsManagement.App.Exceptions;
using AdsManagement.App.Interfaces;
using AdsManagement.App.Interfaces.Service;
using AdsManagement.App.Interfaces.Storage;
using AdsManagement.App.Settings;
using AdsManagement.Domain.Models;
using AutoMapper;
using Microsoft.Extensions.Options;

namespace AdsManagement.App.Services
{
    public class AdvertisementService : IAdvertisementService
    {
        private readonly IAdvertisementStorage _storage;
        private readonly IAdImageService _adImageService;
        private readonly IFileStorageService _fileService;
        private readonly IAccessValidationsService _accessValidations;
        private readonly IMapper _mapper;
        private readonly AdServiceSettings _settings;
        public AdvertisementService(IAdvertisementStorage storage, IMapper mapper, IAdImageService adImageService,
            IFileStorageService fileStorageService, IOptions<AdServiceSettings> options, IAccessValidationsService accessValidations)
        {
            _storage = storage;
            _mapper = mapper;
            _adImageService = adImageService;
            _fileService = fileStorageService;
            _settings = options.Value;
            _accessValidations = accessValidations;
        }
        public async Task<ResponseAdvertisementDto> GetAdvertisementAsync(Guid id, CancellationToken token = default)
        {
            var advertisementDb = await _storage.GetAsync(id, token);
            return _mapper.Map<ResponseAdvertisementDto>(advertisementDb);
        }
        public async Task<Guid> AddAdvertisementAsync(CreateAdvertisementDto advertisementDto, IFileData? file = null, CancellationToken token = default)
        {
            if (advertisementDto == null)
                throw new ArgumentNullException(nameof(advertisementDto), "The advertisement cannot be null");

            if (await IsLimitReached(advertisementDto.UserId, token))
                throw new ExceedingTheAdLimitException("The limit on the allowed number of ads has been reached");

            var ad = _mapper.Map<Advertisement>(advertisementDto);

            var adId = await TryAddWithNextNumberAsync(ad, token);

            if (file is not null)
            {
                try
                {
                    await _adImageService.AddAdImageAsync(adId, file, advertisementDto.UserId, token);
                }
                catch (Exception ex)
                {
                    await _storage.DeleteAsync(adId);
                    throw;
                }
            }
            return adId;
        }
        public async Task DeleteAdvertisementAsync(Guid id, Guid requestUserId, CancellationToken token = default)
        {
            if (requestUserId == Guid.Empty)
                throw new ArgumentException(nameof(requestUserId), "The ID of the user who sent the request cannot be empty");

            await _accessValidations.EnsureAdOwnerAsync(id, requestUserId, token);

            var images = await _adImageService.GetByAdIdAsync(id);

            if (images is not null && images.Count > 0)
            {
                var deleteTasks = images.Select(async image =>
                {
                    await _fileService.DeleteAsync(image.OriginalImagePath, token);
                    await _fileService.DeleteAsync(image.SmallImagePath, token);
                });

                await Task.WhenAll(deleteTasks);
            }
        }
        public async Task UpdateAdvertisementAsync(UpdateAdvertisementDto advertisementDto, Guid requestUserId, CancellationToken token = default)
        {
            if (requestUserId == Guid.Empty)
                throw new ArgumentException(nameof(requestUserId), "The ID of the user who sent request cannot be emprty");

            if (advertisementDto is null)
                throw new ArgumentNullException(nameof(advertisementDto), "The advertisement cannot be null");

            await _accessValidations.EnsureAdOwnerAsync(advertisementDto.Id, requestUserId, token);

            var ad = _mapper.Map<Advertisement>(advertisementDto);

            await _storage.UpdateAsync(ad, token);
        }
        public async Task<PagedResult<ResponseAdvertisementDto>> GetFilterAdsAsync(AdFilterDto filter, CancellationToken token = default)
        {
            var adsDb = await _storage.GetFilterAdsAsync(filter, token);

            return _mapper.Map<PagedResult<ResponseAdvertisementDto>>(adsDb);
        }
        public async Task<PagedResult<ResponseAdvertisementDto>> GetByUserAdsAsync(Guid userId, UserAdvertisementFilterDto filter, CancellationToken token = default)
        {
            var adsDb = await _storage.GetUserAdsAsync(userId, filter, token);
            return _mapper.Map<PagedResult<ResponseAdvertisementDto>>(adsDb);
        }
        public async Task RecalculateRatingAsync(Guid id, CancellationToken token = default)
        {
            await _storage.UpdateRatingAsync(id, token);
        }
        private async Task<bool> IsLimitReached(Guid userId, CancellationToken token = default)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("The user's ID cannot be empty");
            var countAdActice = await _storage.GetUserAdsCountActive(userId, token);

            if (countAdActice >= _settings.LimiteAdvertisement)
                return true;
            return false;
        }
        private async Task<Guid> TryAddWithNextNumberAsync(Advertisement ad, CancellationToken token = default)
        {
            int numberAttempts = 3;
            for (int i = 0; i < numberAttempts; i++)
            {
                try
                {
                    ad.SetNumber(await _storage.GetNextAdNumberAsync(ad.UserId, token));
                    return await _storage.AddAsync(ad, token);

                }
                catch (DuplicateAdNumberException ex) when (i < numberAttempts)
                {
                    if (i == numberAttempts - 1)
                        throw new Exception("Cannot generate unique ad number after 3 attempts");
                }
            }
            throw new Exception("Unexpected error while adding advertisement");
        }

    }
}