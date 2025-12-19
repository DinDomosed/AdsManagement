using AdsManagement.App.Exceptions;
using AdsManagement.App.Exceptions.NotFound;
using AdsManagement.App.Interfaces.Storage;
using AdsManagement.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace AdsManagement.Data.Storages
{
    public class AdImageStorage : IAdImageStorage
    {
        private readonly AdsDbContext _context;
        private readonly int _limitImage;
        public AdImageStorage(AdsDbContext context, int limitImage)
        {
            _context = context;
            _limitImage = limitImage;
        }
        public async Task<Guid> AddAsync(AdvertisementImage image, CancellationToken token = default)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image), "The image cannot be null");

            if (!await AdExists(image.AdvertisementId, token))
                throw new AdvertisementNotFoundException(image.AdvertisementId);

            if (await IsImageLimitReached(image.AdvertisementId, token))
                throw new AdvertisementImageLimitExceededException(image.AdvertisementId, _limitImage);

            _context.AdvertisementImages.Add(image);
            await _context.SaveChangesAsync(token);
            return image.Id;
        }
        public async Task DeleteAsync(Guid id, CancellationToken token = default)
        {
            if (id == Guid.Empty)
                throw new ArgumentException(nameof(id), "The image ID cannot be empty");

            var dbImage = await _context.AdvertisementImages.FindAsync(id, token) ?? throw new AdImageNotFoundException(id);

            _context.AdvertisementImages.Remove(dbImage);
            await _context.SaveChangesAsync(token);
        }
        public async Task<AdvertisementImage> GetAsync(Guid id, CancellationToken token = default)
        {
            if (id == Guid.Empty)
                throw new ArgumentException(nameof(id), "The image ID cannot be empty");

            var image = await _context.AdvertisementImages
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (image == null)
                throw new AdImageNotFoundException(id);

            return image;
        }
        public async Task<List<AdvertisementImage>> GetByAdIdAsync(Guid advertisementId, CancellationToken token = default)
        {
            if (advertisementId == Guid.Empty)
                throw new ArgumentException(nameof(advertisementId), "The advertisement ID cannot be empty");

            if (!await AdExists(advertisementId, token))
                throw new AdvertisementNotFoundException(advertisementId);

            return await _context.AdvertisementImages
                .AsNoTracking()
                .Where(c => c.AdvertisementId == advertisementId)
                .ToListAsync(token);

        }
        private async Task<bool> AdExists(Guid adId, CancellationToken token)
        {
            if (adId == Guid.Empty)
                return false;
            return await _context.Advertisements.AnyAsync(c => c.Id == adId, token);
        }
        private async Task<bool> IsImageLimitReached(Guid adId, CancellationToken token)
        {
            if (adId == Guid.Empty)
                throw new ArgumentException(nameof(adId), "The advertisement ID cannot be empty");
            var query = _context.AdvertisementImages.AsNoTracking().AsQueryable();
            query = query.Where(c => c.AdvertisementId == adId);

            int count = await query.CountAsync(token);

            if (_limitImage > count)
                return false;

            return true;
        }
    }
}
