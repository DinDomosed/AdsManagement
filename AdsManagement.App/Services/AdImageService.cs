using AdsManagement.App.DTOs.AdImage;
using AdsManagement.App.Exceptions;
using AdsManagement.App.Interfaces;
using AdsManagement.App.Interfaces.Service;
using AdsManagement.App.Interfaces.Storage;
using AdsManagement.App.Settings;
using AdsManagement.Domain.Models;
using AutoMapper;
using Microsoft.Extensions.Options;
using System.IO;

namespace AdsManagement.App.Services
{
    public class AdImageService : IAdImageService
    {
        private readonly IAdImageStorage _storage;
        private readonly AdImageServiceSettings _imagesSettings;
        private readonly IMapper _mapper;
        private readonly IImageProcessorService _imageProcessorService;
        private readonly IFileStorageService _fileStorageService;
        private readonly IAccessValidationsService _accessValidations;
        public AdImageService(IAdImageStorage storage, IOptions<AdImageServiceSettings> settings, IMapper mapper,
            IImageProcessorService imageProcessorService, IFileStorageService fileStorageService,
            IAccessValidationsService accessValidations)
        {
            _storage = storage;
            _imagesSettings = settings.Value
                ?? throw new ArgumentNullException(nameof(settings), "The image settings cannot be null");
            _mapper = mapper;
            _imageProcessorService = imageProcessorService;
            _fileStorageService = fileStorageService;
            _accessValidations = accessValidations;
        }
        public async Task<List<ResponseAdImageDto>> GetByAdIdAsync(Guid advertisementId, CancellationToken token = default)
        {
            var images = await _storage.GetByAdIdAsync(advertisementId, token);
            return _mapper.Map<List<ResponseAdImageDto>>(images);
        }
        public async Task<ResponseAdImageDto> GetImageAsync(Guid id, CancellationToken token = default)
        {
            var dbImage = await _storage.GetAsync(id, token);
            return _mapper.Map<ResponseAdImageDto>(dbImage);
        }
        public async Task<Guid> AddAdImageAsync(Guid adId, IFileData file, Guid requestUserId, CancellationToken token = default)
        {
            await _accessValidations.EnsureAdOwnerAsync(adId, requestUserId, token);

            if (await _storage.IsImageLimitReached(adId, _imagesSettings.LimitImage, token))
                throw new AdvertisementImageLimitExceededException(adId, _imagesSettings.LimitImage);

            if (file.Length > (int)_imagesSettings.MaximumSizeMb * 1024 * 1024)
                throw new InvalidFileWeightException(file.Length, _imagesSettings.LimitImage);

            if (!file.ContentType.StartsWith("image/"))
                throw new ArgumentException(nameof(file.ContentType), "The file must be an image.");

            using var stream = file.OpenReadStream();

            stream.Position = 0;

            var guid = Guid.NewGuid();

            var originalFile = $"{guid}.jpg";
            var smallFile = $"{guid}-small.jpg";

            var fullPathOrig = GetFullPath(adId, originalFile);
            var fullPathSmall = GetFullPath(adId, smallFile);

            var adImage = new AdvertisementImage(adId, fullPathOrig, fullPathSmall);
            var IdDb = await _storage.AddAsync(adImage);

            try
            {
                await _fileStorageService.SaveAsync(stream, fullPathOrig, token);
                var compressedStream = await _imageProcessorService.СompressionImageAsync(stream, token);
                await _fileStorageService.SaveAsync(compressedStream, fullPathSmall, token);
            }
            catch (Exception ex)
            {
                await _storage.DeleteAsync(IdDb, token);

                if (_fileStorageService.FileExists(fullPathOrig)) _fileStorageService.DeleteAsync(fullPathOrig);

                if (_fileStorageService.FileExists(fullPathSmall)) _fileStorageService.DeleteAsync(fullPathSmall);

                throw;
            }
            return IdDb;
        }
        public async Task DeleteAdImageAsync(Guid id, Guid requestUserId, CancellationToken token = default)
        {
            if (requestUserId == Guid.Empty)
                throw new ArgumentException(nameof(requestUserId), "The ID of the user who sent the request cannot be empty");

            var imagesDb = await _storage.GetAsync(id, token);

            await _accessValidations.EnsureAdOwnerAsync(imagesDb.AdvertisementId, requestUserId, token);

            await _storage.DeleteAsync(id, token);

            if (_fileStorageService.FileExists(imagesDb.OriginalImagePath)) _fileStorageService.DeleteAsync(imagesDb.OriginalImagePath);
            if (_fileStorageService.FileExists(imagesDb.SmallImagePath)) _fileStorageService.DeleteAsync(imagesDb.SmallImagePath);
        }
        private string GetFullPath(Guid advertisementId, string fileName)
        {
            var directory = Path.Combine(_imagesSettings.ImagesDirectory, advertisementId.ToString());
            var dirInfo = new DirectoryInfo(directory);
            if (!dirInfo.Exists)
                dirInfo.Create();

            return Path.Combine(directory, fileName);
        }
    }
}
